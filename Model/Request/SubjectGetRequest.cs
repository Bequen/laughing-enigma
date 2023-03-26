using System.ComponentModel.DataAnnotations;

namespace Model.Request;

public class SubjectGetRequest
{
    public String? Filter { get; set; }
    [Range(0, 100)]
    public int? Limit { get; set; } = 10;
}