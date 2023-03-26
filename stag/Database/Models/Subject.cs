namespace stag.Database.Models;

public class Subject
{
    public int SubjectId { get; set; }
    public String Name { get; set; }
    public String ShortName { get; set; }
    public String Description { get; set; }
    public int DepartmentId { get; set; }
}