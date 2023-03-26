namespace stag.Database.Models;

/// <summary>
/// Specifies single time block for course
/// </summary>
public class TimetableEventTime
{
    public int TimetableEventTimeId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int TimetableEventId { get; set; }
}