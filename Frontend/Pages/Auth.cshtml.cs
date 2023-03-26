using Frontend.Api.Stag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Model.Request;
using Model.Response;

namespace Frontend.Pages;

public class LoginForm {
    public string Email { get; set; }
    public string Password { get; set; }
}

public class AuthModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public bool IsInvalid { get; set; } = false;

    public AuthModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnPostLoginAsync(LoginForm form) {
        var client = new AuthHandler();
        try {
            var response = await client.Login(new AuthRequest() {
                Email = form.Email,
                Password = form.Password
            });

            if(response == null) {
                return Page();
            }

            /* Create a new cookie containing the authorization token */
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Delete("user_token");
            Response.Cookies.Delete("user_name");
            Response.Cookies.Delete("user_email");
            Response.Cookies.Append("user_token", response.Token, cookieOptions);
            Response.Cookies.Append("user_name", response.Username, cookieOptions);
            Response.Cookies.Append("user_email", response.Email, cookieOptions);

            return Redirect("Index");
        } catch (Exception e) {
            _logger.LogError(e, "Invalid login");
            return Page();
        }
    }

    public void OnGet()
    {
        IsInvalid = Request.Query["invalid"].Any();
    }
}