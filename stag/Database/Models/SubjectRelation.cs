namespace stag.Database.Models;

public enum RelationType {
    Enrolled,
    Garant,
    Lecturer,
    Practicioner
}

/// <summary>
/// Relation of user to some subject.
/// </summary>
public class SubjectRelation
{
    public int SubjectRelationId { get; set; }
    public int SubjectId { get; set; }
    public String UserId { get; set; }
    public RelationType RelationType { get; set; }
}