using System.Net.Http.Headers;

namespace Frontend.Api;

class ApiHandler {
    protected HttpClient client = new HttpClient();
    public string? AuthToken { 
        get => client.DefaultRequestHeaders.Authorization?.Parameter;
        set {
            if(!String.IsNullOrEmpty(value)) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", value);
            } else {
                client.DefaultRequestHeaders.Authorization = null;
            }
        }
    }

    public ApiHandler(String url, string? auth) {
        this.AuthToken = auth;
        
        client.BaseAddress = new Uri("http://localhost:8080/" + url);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        /* Assigns the auth automatically */
        if(!String.IsNullOrEmpty(auth)) {
            AuthToken = auth;
        }
    }

    ~ApiHandler() {
        client.Dispose();
    }

    protected async Task<HttpResponseMessage> Get(String endpoint) {
        Console.WriteLine($"Seding get to: {client.BaseAddress + endpoint}");

        var response = client.GetAsync(endpoint).Result;
        response.EnsureSuccessStatusCode();
        return response;
    }

    protected async Task<HttpResponseMessage> Post<T>(String endpoint, T data) {
        Console.WriteLine($"Sending post to: {client.BaseAddress + endpoint}");
        var response = client.PostAsJsonAsync<T>(endpoint, data).Result;
        response.EnsureSuccessStatusCode();
        return response;
    }

    protected async Task<HttpResponseMessage> Put<T>(String endpoint, T data) {
        Console.WriteLine($"Seding post to: {client.BaseAddress + endpoint}");
        var response = client.PutAsJsonAsync<T>(endpoint, data).Result;
        response.EnsureSuccessStatusCode();
        return response;
    }
}