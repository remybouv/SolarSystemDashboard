using System.Threading;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Interfaces;

/// <summary>
/// Service responsable de la gestion automatique des tokens d'authentification pour l'API le-systeme-solaire.net
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Obtient un token d'authentification valide, en le renouvelant automatiquement si nécessaire
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation pour l'opération async</param>
    /// <returns>Un token d'authentification valide</returns>
    /// <exception cref="System.InvalidOperationException">Lancée si impossible d'obtenir un token valide</exception>
    Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Force le renouvellement du token actuel
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation pour l'opération async</param>
    /// <returns>Le nouveau token généré</returns>
    Task<string> RefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le token actuel et force un renouvellement au prochain appel
    /// </summary>
    void InvalidateCurrentToken();
}