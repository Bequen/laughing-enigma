namespace Model.Response;

public class TimetableEventGetResponse {
    public int TimetableEventId { get; set; }
    public int SubjectId { get; set; }
    public int EventType { get; set; }
    public string OwnerId { get; set; }
}