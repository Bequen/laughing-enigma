using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class RegisterRequest
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}