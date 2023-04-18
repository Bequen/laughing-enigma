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

public class Subjects : PageModel
{
    public List<SubjectGetResponse> subjects { get; set; } = new List<SubjectGetResponse>();
    public SubjectGetResponse? subject { get; set; }

    private PersonHandler personHandler = new PersonHandler();

    private SubjectHandler subjectHandler = new SubjectHandler();

    public List<TimetableEventGetResponse> TimetableEvents { get; set; } = new List<TimetableEventGetResponse>();
    public List<SubjectRelationGetResponse> SubjectRelations { get; set; } = new List<SubjectRelationGetResponse>();

    [BindProperty]
    public SubjectSetForm subjectSetForm { get; set; }

    public async Task<IActionResult> OnPostAddUser(AddUserForm form) {
        subjectHandler.AuthToken = Request.Cookies["user_token"] as string;

        switch(form.Role) {
            case 0:
                await subjectHandler.SetPracticioner(form.SubjectId, form.UserId);
                break;
            case 1:
                await subjectHandler.SetTutor(form.SubjectId, form.UserId);
                break;
        }
        return Page();
    }

    /// <summary>
    /// On posting the changing form
    /// </summary>
    /// <param name="form">Form values containing info about subject.</param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAsync(SubjectSetForm form) {

        Console.WriteLine("Updating subject");
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

        if(RouteData.Values["subjectId"] != null) {
            subject = subjects.FirstOrDefault(x => x.SubjectId == int.Parse((string)(RouteData.Values["subjectId"])));
        
            TimetableEvents = (await subjectHandler.GetTimetableEvents(subject.SubjectId)).ToList();
            SubjectRelations = (await subjectHandler.GetSubjectRelations(subject.SubjectId)).ToList();
        }

        return Page();
    }
}