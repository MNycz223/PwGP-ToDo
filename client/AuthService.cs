using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Blazored.SessionStorage;

namespace client;

public class AuthService
{
    private readonly HttpClient _client;
    private readonly ISessionStorageService _sessionStorage;

    private const string AuthTokenKey = "authToken";

    public AuthService(HttpClient httpClient, ISessionStorageService sessionStorage)
    {
        _client = httpClient;
        _sessionStorage = sessionStorage;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var response = await _client.GetAsync("/login");

        if (response.IsSuccessStatusCode)
        {
            await _sessionStorage.SetItemAsync(AuthTokenKey, authHeader);
            await _sessionStorage.SetItemAsync("username", username);
            return true;
        }

        await LogoutAsync();
        return false;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        string json = JsonSerializer.Serialize(new { username = username, password = passwordHash });
        StringContent query = new(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/user", query);
        return response.IsSuccessStatusCode;
    }

    public async Task RestoreLoginAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>(AuthTokenKey);

        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
    }

    public async Task<bool> IsLoggedAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>(AuthTokenKey);
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task LogoutAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        await _sessionStorage.ClearAsync();
    }
}
