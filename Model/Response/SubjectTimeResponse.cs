namespace Model.Response;

public class SubjectTimeResponse {
    public int SubjectId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int TimetableEventId { get; set; }
}