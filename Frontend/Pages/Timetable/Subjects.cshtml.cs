using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Response;
using Model.Request;
using Microsoft.AspNetCore.Mvc;
using Frontend.Api.Stag;
using System.Net;

namespace Frontend.Pages.Timetable;

public class SubjectTimeForm {
    public int Type { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class SubjectSetForm {
    public int SubjectId { get; set; }
    public String Name { get; set; }
    public String ShortName { get; set; }
    public String Description { get; set; }

    public IEnumerable<SubjectTimeForm> Times { get; set; }
}

public class AddUserForm {
    public int SubjectId { get; set; }
    public string UserId { get; set; }
    public int Role { get; set; }
}

public class AddTimeForm {
    public int SubjectId { get; set; }
    public int EventId { get; set; }

    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public int Frequency { get; set; }
}

public class Subjects : PageModel
{
    public List<SubjectGetResponse> subjects { get; set; } = new List<SubjectGetResponse>();
    public SubjectGetResponse? subject { get; set; }

    private PersonHandler personHandler = new PersonHandler();

    private SubjectHandler subjectHandler = new SubjectHandler();

    public TimetableEventGetResponse Event { get; set; } = null;
    public List<TimetableEventGetResponse> TimetableEvents { get; set; } = new List<TimetableEventGetResponse>();
    public List<SubjectRelationGetResponse> SubjectRelations { get; set; } = new List<SubjectRelationGetResponse>();

    public List<TimetableEventTimeGetResponse> EventTimes { get; set; } = new List<TimetableEventTimeGetResponse>();

    [BindProperty]
    public SubjectSetForm subjectSetForm { get; set; }

    public async Task<IActionResult> OnPostAddUser(AddUserForm form) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;

        try {
            switch(form.Role) {
                case 0:
                    await subjectHandler.SetPracticioner(form.SubjectId, form.UserId);
                    break;
                case 1:
                    await subjectHandler.SetTutor(form.SubjectId, form.UserId);
                    break;
            }
        } catch {

        }
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteEvent(int subjectId, int eventId) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
        try {
            await subjectHandler.DeleteTimetableEvent(subjectId, eventId);
        } catch {
            
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddTimetableEvent(int subjectId, int type) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
        
        try {
            switch(type) {
                case 0:
                    await subjectHandler.AddLectureEvent(subjectId);
                    break;
                case 1:
                    await subjectHandler.AddPractiseEvent(subjectId);
                    break;
            }
        } catch(HttpRequestException e) {
            
        } catch(Exception e) {
            
        }


        return Page();
    }

    public async Task<IActionResult> OnPostAddTime(AddTimeForm form) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;

        try {
            await subjectHandler.AddTime(form.SubjectId, form.EventId, new List<SubjectSetTimeRequest>() {
                new SubjectSetTimeRequest() {
                    StartsAt = form.StartDate.ToDateTime(form.StartTime),
                    EndsAt = form.StartDate.ToDateTime(form.EndTime),
                    Frequence = form.Frequency
                }
            });
        } catch {

        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteEventTime(int subjectId, int eventId, int eventTimeId) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
        try {
            await subjectHandler.DeleteTimetableEventTime(subjectId, eventId, eventTimeId);
        } catch {

        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteSubject(int subjectId) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
        try {
            await subjectHandler.DeleteSubject(subjectId);
        } catch {

        }

        return Page();
    }

    /// <summary>
    /// On posting the changing form
    /// </summary>
    /// <param name="form">Form values containing info about subject.</param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAsync(SubjectSetForm form) {

        try {
            SubjectSetRequest request = new SubjectSetRequest() {
                Name = form.Name,
                ShortName = form.ShortName,
                Description = form.Description
            };
            subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
            await subjectHandler.Set(form.SubjectId, request);
        } catch(HttpRequestException e) {
            if(e.StatusCode == HttpStatusCode.Unauthorized) {
                return RedirectToPage("/Auth");
            }
        } catch {
            throw;
        }

        return Page();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        personHandler.AuthToken = Request.Cookies["user_token"] as string;
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;
        try {
            subjects = (await personHandler.GetSubjects()).ToList() ?? new List<SubjectGetResponse>();
        } catch (HttpRequestException e) {
            if(e.StatusCode == HttpStatusCode.Unauthorized) {
                return RedirectToPage("/Auth");
            }
        } catch {
            throw;
        }

        int subjectId = 0;
        if(RouteData.Values["subjectId"] != null && int.TryParse((string)(RouteData.Values["subjectId"]), out subjectId)) {
            subject = subjects.FirstOrDefault(x => x.SubjectId == subjectId);
            
            if(subject == null) {
                return Page();
            }
            TimetableEvents = (await subjectHandler.GetTimetableEvents(subject.SubjectId)).ToList();
            SubjectRelations = (await subjectHandler.GetSubjectRelations(subject.SubjectId)).ToList();
            Console.WriteLine($"Relations {SubjectRelations.Count}");
        }

        if(RouteData.Values["eventId"] != null) {
            Event = TimetableEvents.FirstOrDefault(x => x.TimetableEventId == int.Parse((string)(RouteData.Values["eventId"])));

            if(Event != null)
                EventTimes = (await subjectHandler.GetTimetableEventTimes(subject.SubjectId, Event.TimetableEventId)).ToList();

                Console.WriteLine($"Event Times: {EventTimes.Count}");
        }

        return Page();
    }
}