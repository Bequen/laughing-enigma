using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class SubjectSetRequest
{
    [Required]
    public String Name { get; set; }
    [Required]
    public String ShortName { get; set; }
    [Required]
    public String Description { get; set; }

}