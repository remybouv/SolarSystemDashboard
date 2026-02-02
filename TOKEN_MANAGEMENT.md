# 🔐 Gestion des Tokens API - SolarSystemDashboard

Ce document décrit le système de gestion automatique des tokens pour l'API le-systeme-solaire.net.

## 🎯 Fonctionnalités

- ✅ **Injection de dépendance** avec `ITokenService`
- ✅ **Configuration flexible** via appsettings.json, user secrets ou variables d'environnement
- ✅ **Retry logic robuste** avec gestion d'erreurs
- ✅ **Logging détaillé** pour le debugging
- ✅ **Cache de token** pour éviter les appels inutiles
- ✅ **Gestion thread-safe** avec semaphore

## 📁 Fichiers créés/modifiés

### Nouveaux fichiers
- `Interfaces/ITokenService.cs` - Interface du service de gestion de token
- `Services/TokenService.cs` - Implémentation avec retry logic et cache
- `Models/ApiConfiguration.cs` - Configuration typée pour l'API
- `appsettings.json` - Configuration par défaut

### Fichiers modifiés
- `Services/BodiesService.cs` - Intégration du `ITokenService`
- `Extensions/ServiceCollectionExtensions.cs` - Configuration DI + logging
- `SolarSystemDashboard.csproj` - Packages NuGet pour configuration/logging

## ⚙️ Configuration

### appsettings.json
```json
{
  "Api": {
    "BaseUrl": "https://api.le-systeme-solaire.net",
    "TokenGenerationUrl": "https://api.le-systeme-solaire.net/generatekey.html",
    "DefaultToken": "a2becb0c-4476-42fc-b92c-a58974c4094c",
    "HttpTimeoutSeconds": 30,
    "MaxRetryAttempts": 3,
    "RetryDelayMs": 1000
  }
}
```

### User Secrets (recommandé pour production)
```bash
dotnet user-secrets set "Api:DefaultToken" "votre-nouveau-token-ici"
```

### Variables d'environnement
```bash
export SOLARSYSTEM_Api__DefaultToken="votre-nouveau-token-ici"
```

## 🔧 Utilisation

Le `TokenService` est automatiquement injecté dans `BodiesService`. Aucun changement de code n'est nécessaire dans les ViewModels ou autres services.

```csharp
// Injection automatique
public class BodiesService : IBodiesService
{
    public BodiesService(HttpClient httpClient, ITokenService tokenService, ILogger<BodiesService> logger)
    {
        // Le TokenService gère automatiquement l'authentification
    }
}
```

## 🔄 Génération de nouveaux tokens

Pour obtenir un nouveau token :

1. **Méthode manuelle** (actuelle) :
   - Visiter https://api.le-systeme-solaire.net/generatekey.html
   - Entrer votre email
   - Copier le token généré
   - Le configurer via user secrets ou variables d'environnement

2. **Méthode automatique** (future) :
   - Le `TokenService` pourra être étendu pour automatiser ce processus
   - Nécessiterait l'implémentation d'un formulaire web automatisé

## 📝 Logging

Le système utilise le logging Microsoft.Extensions.Logging avec différents niveaux :

- **Information** : Opérations importantes (génération, utilisation de fallback)
- **Debug** : Détails techniques (cache hits, timings)
- **Warning** : Problèmes non critiques (retry attempts)
- **Error** : Erreurs critiques nécessitant une intervention

## 🧪 Tests

Un test simple est disponible dans `TokenServiceTest.cs` :

```csharp
await TokenServiceTest.TestTokenGeneration();
```

## 🔒 Sécurité

- ✅ Tokens stockés en mémoire uniquement
- ✅ Configuration via user secrets supportée
- ✅ Variables d'environnement avec préfixe `SOLARSYSTEM_`
- ✅ Logging masque les tokens complets (affiche seulement les 8 premiers caractères)

## 🚀 Déploiement

1. **Développement** : Utiliser appsettings.json avec le token par défaut
2. **Production** : Configurer via user secrets ou variables d'environnement
3. **CI/CD** : Variables d'environnement avec préfixe `SOLARSYSTEM_`

## 🔮 Évolutions futures possibles

- [ ] Automatisation complète de la génération de token
- [ ] Cache persistant avec encryption
- [ ] Monitoring et alertes sur expiration
- [ ] Interface utilisateur pour gestion des tokens
- [ ] Support de multiple tokens/environnements