using System.Net.Http.Headers;
using System.Text;

namespace client;

public class AuthService
{
    private readonly HttpClient _client;

    public AuthService(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var response = await _client.GetAsync("/v1/user/username");

        return response.IsSuccessStatusCode;
    }
}
