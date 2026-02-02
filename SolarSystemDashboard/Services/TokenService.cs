using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Models;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Services;

/// <summary>
/// Service de gestion automatique des tokens pour l'API le-systeme-solaire.net
/// </summary>
public sealed partial class TokenService : ITokenService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ApiConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    private string? _currentToken;
    private DateTime _lastTokenRefresh = DateTime.MinValue;
    private bool _disposed;

    [GeneratedRegex(@"[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex UuidPattern();

    public TokenService(HttpClient httpClient, IOptions<ApiConfiguration> configuration, ILogger<TokenService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation("TokenService initialisé avec URL de base: {BaseUrl}", _configuration.BaseUrl);
    }

    /// <inheritdoc />
    public async Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TokenService));

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Si nous avons déjà un token, le retourner
            if (!string.IsNullOrEmpty(_currentToken))
            {
                _logger.LogDebug("Utilisation du token en cache");
                return _currentToken;
            }

            // Sinon, en générer un nouveau
            return await RefreshTokenInternalAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TokenService));

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await RefreshTokenInternalAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public void InvalidateCurrentToken()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TokenService));

        lock (this)
        {
            _currentToken = null;
            _lastTokenRefresh = DateTime.MinValue;
        }
        
        _logger.LogInformation("Token actuel invalidé");
    }

    private async Task<string> RefreshTokenInternalAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Demande de renouvellement de token...");

        // Pour l'instant, l'API le-systeme-solaire.net nécessite une génération manuelle via interface web
        // Nous utilisons le token configuré comme fallback et logguons pour information
        if (!string.IsNullOrEmpty(_configuration.DefaultToken))
        {
            _logger.LogInformation("Utilisation du token configuré (renouvellement manuel requis via {Url})", _configuration.TokenGenerationUrl);
            _currentToken = _configuration.DefaultToken;
            _lastTokenRefresh = DateTime.UtcNow;
            return _configuration.DefaultToken;
        }

        // Tentative d'appel de l'API pour documenter la réponse (à des fins de débogage)
        try
        {
            _logger.LogDebug("Appel de l'API de génération pour analyse: {Url}", _configuration.TokenGenerationUrl);
            var response = await _httpClient.GetAsync(_configuration.TokenGenerationUrl, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("Réponse API reçue, longueur: {Length} caractères", content.Length);
                
                // Tenter d'extraire un token s'il y en a un
                var extractedToken = ExtractTokenFromHtml(content);
                if (!string.IsNullOrEmpty(extractedToken))
                {
                    _logger.LogInformation("Token extrait de l'API: {TokenPrefix}...", extractedToken[..8]);
                    _currentToken = extractedToken;
                    _lastTokenRefresh = DateTime.UtcNow;
                    return extractedToken;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors de l'appel de l'API de génération");
        }

        _logger.LogError("Aucun token disponible - veuillez configurer un token valide");
        throw new InvalidOperationException("Aucun token d'authentification disponible. Veuillez en configurer un via appsettings.json ou user secrets.");
    }

    private static string? ExtractTokenFromHtml(string htmlContent)
    {
        // L'API retourne probablement le token dans le HTML
        // Chercher un pattern UUID dans le contenu
        var match = UuidPattern().Match(htmlContent);
        return match.Success ? match.Value : null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _semaphore.Dispose();
        
        _logger.LogDebug("TokenService disposé");
    }
}