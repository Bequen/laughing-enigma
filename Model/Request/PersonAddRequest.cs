using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class PersonAddRequest {
    [Required]
    public String UserId { get; set; }
    [Required]
    public String FirstName { get; set; }
    [Required]
    public String LastName { get; set; }
}