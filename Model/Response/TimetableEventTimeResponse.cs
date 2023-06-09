namespace Model.Response;

public class TimetableEventTimeGetResponse {
    public int TimetableEventTimeId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
}

public class TimetableEventTimeResponse {
    public int TimetableEventTimeId { get; set; }
    public TimetableEventGetResponse Event { get; set; }
    public SubjectGetResponse Subject { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
}