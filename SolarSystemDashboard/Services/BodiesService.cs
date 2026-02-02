using Microsoft.Extensions.Logging;
using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Services;

/// <summary>
/// Service responsable de l'accès aux données des corps célestes depuis l'API le-systeme-solaire.net
/// </summary>
public class BodiesService : IBodiesService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<BodiesService> _logger;

    public BodiesService(HttpClient httpClient, ITokenService tokenService, ILogger<BodiesService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Configure l'authentification Bearer avec un token valide
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation pour l'opération async</param>
    /// <returns>True si l'authentification a été configurée avec succès</returns>
    private async Task<bool> SetBearerTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _tokenService.GetValidTokenAsync(cancellationToken);
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Token d'authentification vide reçu du TokenService");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Token d'authentification configuré avec succès");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la configuration du token d'authentification");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<Bodies> GetBodiesAsync(CancellationToken cancellationToken = default)
    {
        var maxRetries = 2;
        var attempt = 0;

        while (attempt <= maxRetries)
        {
            try
            {
                attempt++;
                _logger.LogDebug("Tentative {Attempt} de récupération des corps célestes", attempt);

                if (!await SetBearerTokenAsync(cancellationToken))
                {
                    throw new UnauthorizedAccessException("Impossible de configurer l'authentification");
                }

                var response = await _httpClient.GetAsync("/rest/bodies", cancellationToken);
                
                // Si on reçoit une 401 et qu'on n'a pas encore fait de retry, invalider le token et réessayer
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && attempt <= maxRetries)
                {
                    _logger.LogWarning("Réponse 401 reçue, invalidation du token et nouvelle tentative");
                    _tokenService.InvalidateCurrentToken();
                    continue;
                }

                response.EnsureSuccessStatusCode();
                var bodies = await response.Content.ReadFromJsonAsync<Bodies>(cancellationToken);
                
                if (bodies == null)
                {
                    throw new InvalidOperationException("Réponse JSON invalide reçue de l'API");
                }

                _logger.LogInformation("Corps célestes récupérés avec succès");
                return bodies;
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("Échec d'authentification après {Attempt} tentatives", attempt);
                throw;
            }
            catch (HttpRequestException ex) when (attempt <= maxRetries)
            {
                _logger.LogWarning(ex, "Erreur HTTP lors de la tentative {Attempt}, nouvelle tentative...", attempt);
                
                // Petite pause avant retry
                await Task.Delay(1000, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur HTTP finale après {Attempt} tentatives", attempt);
                throw new Exception("Erreur lors de la récupération des corps célestes.", ex);
            }
        }

        throw new Exception($"Impossible de récupérer les corps célestes après {maxRetries + 1} tentatives");
    }
}
