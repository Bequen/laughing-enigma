using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class SubjectPutRequest
{
    [Required]
    public String Name { get; set; }
    [Required]
    public String ShortName { get; set; }
    [Required]
    public String Description { get; set; }

    [Required]
    public String GarantUserId { get; set; }
}