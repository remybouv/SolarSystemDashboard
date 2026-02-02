using SolarSystemDashboard.Interfaces;
using SolarSystemDashboard.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Services;

public class BodiesService : IBodiesService
{
    private readonly HttpClient _httpClient;

    public BodiesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<bool> SetBearerToken()
    {
        var token = "a2becb0c-4476-42fc-b92c-a58974c4094c";
        if (string.IsNullOrEmpty(token))
            return false;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task<Bodies> GetBodiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await SetBearerToken())
            {
                throw new UnauthorizedAccessException("Not authenticated");
            }
            var response = await _httpClient.GetAsync("/rest/bodies", cancellationToken);
            response.EnsureSuccessStatusCode();

            var bodies = await response.Content.ReadFromJsonAsync<Bodies>(cancellationToken);
            return bodies;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("An error occurred while fetching bodies.", ex);
        }
    }
}
