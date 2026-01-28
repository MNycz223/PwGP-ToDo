using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.SessionStorage;
using common.Models;

namespace client;

public class AuthService
{
    private readonly HttpClient _client;
    private readonly ISessionStorageService _sessionStorage;

    private const string AuthTokenKey = "authToken";
    private const string UsernameKey = "username";

    public AuthService(HttpClient httpClient, ISessionStorageService sessionStorage)
    {
        _client = httpClient;
        _sessionStorage = sessionStorage;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var authHeader = await GenerateTokenAsync(username, password);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var response = await _client.GetAsync("/login");

        if (response.IsSuccessStatusCode)
        {
            await _sessionStorage.SetItemAsync(AuthTokenKey, authHeader);
            await _sessionStorage.SetItemAsync(UsernameKey, username);
            return true;
        }

        await LogoutAsync();
        return false;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        string json = JsonSerializer.Serialize(new { username = username, password = password });
        StringContent query = new(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/user", query);
        return response.IsSuccessStatusCode;
    }

    public async Task RestoreLoginAsync(HttpClient client)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        var token = await _sessionStorage.GetItemAsync<string>(AuthTokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
    }

    public async Task GenerateNewTokenAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        var username = await _sessionStorage.GetItemAsync<string>(UsernameKey);
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException("No username stored in session.");

        var token = await GenerateTokenAsync(username, password);
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            await _sessionStorage.SetItemAsync(AuthTokenKey, token);
        }
    }

    private async Task<string> GenerateTokenAsync(string username, string password)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }

    public async Task<bool> IsLoggedAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>(AuthTokenKey);
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<UserModel> GetLoggedUserAsync()
    {
        var username = await _sessionStorage.GetItemAsync<string>(UsernameKey);
        return new() { Username = username };
    }

    public async Task LogoutAsync()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        await _sessionStorage.ClearAsync();
    }
}
