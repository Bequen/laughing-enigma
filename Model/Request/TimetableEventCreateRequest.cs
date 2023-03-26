namespace Model.Request;

public class TimetableEventCreateRequest {
    public int SubjectId { get; set; }
    public int EventType { get; set; }
    public string OwnerId { get; set; }
}