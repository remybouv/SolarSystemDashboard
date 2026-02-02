using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarSystemDashboard.Extensions;
using SolarSystemDashboard.Interfaces;
using System;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Services;

/// <summary>
/// Classe de test simple pour valider le TokenService
/// </summary>
public static class TokenServiceTest
{
    public static async Task TestTokenGeneration()
    {
        // Configuration de test
        var services = new ServiceCollection();
        services.AddCommonServices();
        
        var serviceProvider = services.BuildServiceProvider();
        
        try
        {
            var tokenService = serviceProvider.GetRequiredService<ITokenService>();
            var logger = serviceProvider.GetRequiredService<ILogger<TokenServiceTest>>();
            
            logger.LogInformation("=== Test du TokenService ===");
            
            // Test 1: Obtenir un token
            logger.LogInformation("Test 1: Génération de token...");
            var token = await tokenService.GetValidTokenAsync();
            logger.LogInformation("Token généré: {Token}", token);
            
            // Test 2: Vérifier la mise en cache
            logger.LogInformation("Test 2: Vérification du cache...");
            var cachedToken = await tokenService.GetValidTokenAsync();
            var isFromCache = token == cachedToken;
            logger.LogInformation("Token du cache: {IsFromCache} ({Token})", isFromCache ? "OUI" : "NON", cachedToken);
            
            // Test 3: Force refresh
            logger.LogInformation("Test 3: Renouvellement forcé...");
            var refreshedToken = await tokenService.RefreshTokenAsync();
            logger.LogInformation("Token renouvelé: {Token}", refreshedToken);
            
            // Test 4: Invalidation
            logger.LogInformation("Test 4: Invalidation du token...");
            tokenService.InvalidateCurrentToken();
            var newToken = await tokenService.GetValidTokenAsync();
            logger.LogInformation("Nouveau token après invalidation: {Token}", newToken);
            
            logger.LogInformation("=== Tests terminés avec succès ===");
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TokenServiceTest>>();
            logger.LogError(ex, "Erreur durante les tests");
            throw;
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }
}