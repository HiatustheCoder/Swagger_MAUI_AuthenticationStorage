
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AuthenticationStorageMobile.Models;

namespace AuthenticationStorageMobile.Services;

public class ApiService
{
    private readonly HttpClient _client = new();

    private readonly string _baseUrl =
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5251"
            : "http://localhost:5251";

    // Authenticate using HTTP Basic Authentication
    public async Task<bool> LoginAsync(
        string username, string password)
    {
        string credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{username}:{password}"));

        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"{_baseUrl}/api/auth/login");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        using var response = await _client.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    // Retrieve all stored items
    public async Task<List<Item>> GetItemsAsync()
    {
        return await _client.GetFromJsonAsync<List<Item>>(
            $"{_baseUrl}/api/items") ?? new List<Item>();
    }

    // Save an item through the API
    public async Task<bool> SaveItemAsync(Item item)
    {
        using var response = await _client.PostAsJsonAsync(
            $"{_baseUrl}/api/items", item);

        return response.IsSuccessStatusCode;
    }
}
