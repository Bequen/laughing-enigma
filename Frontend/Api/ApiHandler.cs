using System.Net.Http.Headers;

namespace Frontend.Api;

class ApiHandler {
    protected HttpClient client = new HttpClient();
    public string AuthToken { 
        get => client.DefaultRequestHeaders.Authorization.Parameter;
        set => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", value);
    }

    public ApiHandler(Uri baseAddress, string auth) {
        this.AuthToken = auth;
        client.BaseAddress = baseAddress;
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
        var response = client.GetAsync(endpoint).Result;
        response.EnsureSuccessStatusCode();
        return response;
    }

    protected async Task<HttpResponseMessage> Post<T>(String endpoint, T data) {
        var response = client.PostAsJsonAsync<T>(endpoint, data).Result;
        response.EnsureSuccessStatusCode();
        return response;
    }
}