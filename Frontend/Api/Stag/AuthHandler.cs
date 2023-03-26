namespace Frontend.Api.Stag;

using Model.Request;
using Model.Response;

class AuthHandler : ApiHandler {
    public AuthHandler(string? auth = null) : base(new Uri("http://backend:80/Auth/"), auth) {

    }

    public async Task<AuthResponse?> Login(AuthRequest request) {
        var response = await Post<AuthRequest>("Login", request);
        var json = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if(json != null) {
            return json;
        } else {
            throw new Exception("Failed to parse server response");
        }
    }
}