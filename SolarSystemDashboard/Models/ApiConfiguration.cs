namespace SolarSystemDashboard.Models;

/// <summary>
/// Configuration pour l'API le-systeme-solaire.net
/// </summary>
public sealed record ApiConfiguration
{
    /// <summary>
    /// URL de base de l'API le-systeme-solaire.net
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.le-systeme-solaire.net";

    /// <summary>
    /// URL pour générer un nouveau token d'authentification
    /// </summary>
    public string TokenGenerationUrl { get; init; } = "https://api.le-systeme-solaire.net/generatekey.html";

    /// <summary>
    /// Token d'authentification par défaut (fallback)
    /// </summary>
    public string? DefaultToken { get; init; }

    /// <summary>
    /// Délai d'expiration pour les requêtes HTTP en secondes
    /// </summary>
    public int HttpTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Nombre maximum de tentatives pour renouveler un token
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Délai entre les tentatives de renouvellement en millisecondes
    /// </summary>
    public int RetryDelayMs { get; init; } = 1000;
}