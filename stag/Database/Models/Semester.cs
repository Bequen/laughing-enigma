
namespace Stag.Database.Models;

public enum Season {
    Winter,
    Summer
}

/// <summary>
/// Time block of a single semester
/// </summary>
public class Semester {
    public int SemesterId { get; set; }

    public Season Season { get; set; }

    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }

    public List<SemesterEvent> Events { get; set; }

    public int GetWeekCount() {
        return (int)(EndsAt - StartsAt).TotalDays / 7;
    }
}