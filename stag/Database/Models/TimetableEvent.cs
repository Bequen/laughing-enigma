using System.ComponentModel.DataAnnotations;

namespace Stag.Database.Models;

public enum TimetableEventType {
    Lecture,
    Practice,
    Seminar
}

public class TimetableEvent {
    [Key]
    public int TimetableEventId { get; set; }
    public int SubjectId { get; set; }
    public TimetableEventType EventType { get; set; }
    public string OwnerId { get; set; }
}