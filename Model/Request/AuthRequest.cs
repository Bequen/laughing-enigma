using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class AuthRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}