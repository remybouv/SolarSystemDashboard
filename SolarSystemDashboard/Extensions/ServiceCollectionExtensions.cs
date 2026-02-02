using Microsoft.Extensions.DependencyInjection;
using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Services;
using SolarSystemDashboard.ViewModels;
using System;

namespace SolarSystemDashboard.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainWindowViewModel>();

        var apiBaseUrl = "https://api.le-systeme-solaire.net/rest";
        // Ensure Microsoft.Extensions.Http is referenced in your project
        collection.AddHttpClient<IBodiesService, BodiesService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }
}
