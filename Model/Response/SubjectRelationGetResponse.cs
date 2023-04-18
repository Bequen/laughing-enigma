using Model.Response;

public class SubjectRelationGetResponse {
    public int SubjectId { get; set; }
    public PersonGetResponse Person { get; set; }
    public int RelationType { get; set; }
}