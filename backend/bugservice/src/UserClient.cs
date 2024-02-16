public class UserClient {
    private readonly HttpClient _client;

    public UserClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;

        string baseAddress = configuration["UserService:Url"] ?? string.Empty;
        if (!string.IsNullOrEmpty(baseAddress))
        {
            _client.BaseAddress = new Uri(baseAddress);
        }
    }

    public async Task<bool> Exists(int id)
    {
        var response = await _client.GetAsync($"users/{id}");
        return response.IsSuccessStatusCode;
    }
}