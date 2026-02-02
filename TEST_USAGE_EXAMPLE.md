# 🧪 Exemple d'utilisation - Système TokenService

Voici comment utiliser le nouveau système de gestion de token dans votre application.

## 📁 Structure des fichiers créés

```
SolarSystemDashboard/
├── Interfaces/
│   └── ITokenService.cs           # Interface du service de token
├── Services/
│   ├── TokenService.cs            # Implémentation avec cache et retry
│   └── BodiesService.cs           # Modifié pour utiliser ITokenService
├── Models/
│   └── ApiConfiguration.cs       # Configuration typée
├── Extensions/
│   └── ServiceCollectionExtensions.cs # Configuration DI mise à jour
├── appsettings.json               # Configuration par défaut
└── SolarSystemDashboard.csproj    # Packages NuGet ajoutés
```

## ⚡ Utilisation dans une ViewModel

```csharp
// Dans MainWindowViewModel ou autre
public class MainWindowViewModel : ViewModelBase
{
    private readonly IBodiesService _bodiesService;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(IBodiesService bodiesService, ILogger<MainWindowViewModel> logger)
    {
        _bodiesService = bodiesService;
        _logger = logger;
    }

    public async Task LoadPlanetsAsync()
    {
        try
        {
            // Le TokenService gère automatiquement l'authentification
            var bodies = await _bodiesService.GetBodiesAsync();
            
            _logger.LogInformation("Chargement réussi: {Count} corps célestes", bodies.BodyList?.Count ?? 0);
            
            // Traiter les données...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des planètes");
            // Gérer l'erreur...
        }
    }
}
```

## 🔧 Configuration pour différents environnements

### Développement (appsettings.json)
```json
{
  "Api": {
    "DefaultToken": "a2becb0c-4476-42fc-b92c-a58974c4094c"
  }
}
```

### Production (User Secrets)
```bash
dotnet user-secrets set "Api:DefaultToken" "votre-token-prod-ici"
dotnet user-secrets set "Api:HttpTimeoutSeconds" "60"
```

### CI/CD (Variables d'environnement)
```bash
export SOLARSYSTEM_Api__DefaultToken="token-cicd"
export SOLARSYSTEM_Api__MaxRetryAttempts="5"
```

## 🔄 Gestion manuelle du token

```csharp
// Si vous avez besoin de forcer un renouvellement
public class TokenManagementService
{
    private readonly ITokenService _tokenService;
    
    public async Task RefreshTokenManuallyAsync()
    {
        // Invalider le token actuel
        _tokenService.InvalidateCurrentToken();
        
        // Forcer un nouveau token
        var newToken = await _tokenService.RefreshTokenAsync();
        Console.WriteLine($"Nouveau token: {newToken[..8]}...");
    }
}
```

## 📊 Monitoring et logging

Le système log automatiquement :

- **Info** : Génération/utilisation de token
- **Debug** : Cache hits, timings
- **Warning** : Retry attempts, fallback usage
- **Error** : Échecs critiques

```csharp
// Exemple de sortie log
[14:30:15] [Information] TokenService: Token généré avec succès: a2becb0c...
[14:30:16] [Debug] TokenService: Utilisation du token en cache
[14:35:20] [Warning] BodiesService: Réponse 401 reçue, invalidation du token
[14:35:21] [Information] TokenService: Token généré avec succès: b3c5f8e1...
```

## 🎯 Avantages de cette implémentation

✅ **Zéro modification** des ViewModels existants
✅ **Configuration flexible** par environnement  
✅ **Retry automatique** sur erreur 401
✅ **Thread-safe** avec semaphore
✅ **Logging détaillé** pour debugging
✅ **Memory-safe** avec Dispose pattern
✅ **Type-safe** avec nullable reference types