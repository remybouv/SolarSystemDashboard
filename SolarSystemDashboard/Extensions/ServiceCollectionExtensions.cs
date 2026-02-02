using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Models;
using SolarSystemDashboard.Services;
using SolarSystemDashboard.ViewModels;
using System;
using System.IO;

namespace SolarSystemDashboard.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        // Configuration
        var configuration = BuildConfiguration();
        collection.AddSingleton<IConfiguration>(configuration);
        
        // Logging
        collection.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        });

        // Configuration API
        collection.Configure<ApiConfiguration>(configuration.GetSection("Api"));

        // ViewModels
        collection.AddTransient<MainWindowViewModel>();

        // TokenService avec son propre HttpClient
        collection.AddHttpClient<ITokenService, TokenService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // BodiesService avec son HttpClient configuré
        collection.AddHttpClient<IBodiesService, BodiesService>((serviceProvider, client) =>
        {
            var apiConfig = configuration.GetSection("Api").Get<ApiConfiguration>() ?? new ApiConfiguration();
            client.BaseAddress = new Uri($"{apiConfig.BaseUrl}/rest");
            client.Timeout = TimeSpan.FromSeconds(apiConfig.HttpTimeoutSeconds);
        });
    }

    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(GetApplicationDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Ajouter les user secrets en mode développement si disponible
        try
        {
            builder.AddUserSecrets<Program>();
        }
        catch
        {
            // User secrets non disponibles, ignorer silencieusement
        }

        // Variables d'environnement pour override
        builder.AddEnvironmentVariables("SOLARSYSTEM_");

        return builder.Build();
    }

    private static string GetApplicationDirectory()
    {
        // Obtenir le répertoire de l'exécutable
        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(assemblyLocation);
        
        // Si on est dans un contexte de publication single-file, utiliser le répertoire de travail
        if (string.IsNullOrEmpty(directory) || !File.Exists(Path.Combine(directory, "appsettings.json")))
        {
            directory = Directory.GetCurrentDirectory();
        }
        
        return directory ?? Directory.GetCurrentDirectory();
    }
}
