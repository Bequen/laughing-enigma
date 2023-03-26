namespace Stag.Database.Models;

public enum SemesterEventType {
    Classes,
    Enrollment,
    TimetableScheduling,
    Holiday
}

public class SemesterEvent {
    public int SemesterEventId { get; set; }
    public int SemesterId { get; set; }
    public DateTime EventStarts { get; set; }
    public DateTime EventEnds { get; set; }

    public SemesterEventType EventType { get; set; }
    public String Description { get; set; }
}