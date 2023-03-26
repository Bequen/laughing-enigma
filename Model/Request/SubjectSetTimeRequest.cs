
namespace Model.Request;

public class SubjectSetTimeRequest {
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }

    public int? Frequence { get; set; }
}