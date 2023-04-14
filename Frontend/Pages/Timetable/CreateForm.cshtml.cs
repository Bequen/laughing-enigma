using Frontend.Api.Stag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Request;
using Model.Response;

namespace Frontend.Pages.Timetable;

public class CreateForm : PageModel
{
    [BindProperty]
    public SubjectPutRequest subjectPutForm { get; set; }

    private DepartmentHandler departmentHandler = new DepartmentHandler();
    private PersonHandler personHandler = new PersonHandler();

    private SubjectHandler subjectHandler = new SubjectHandler();

    public List<PersonGetResponse> People { get; set; }

    public async Task<IActionResult> OnPostAsync() {
        Console.WriteLine(subjectPutForm.GarantUserId);

        Console.WriteLine($"{subjectPutForm.Name}");
        Console.WriteLine($"{subjectPutForm.ShortName}");
        Console.WriteLine($"{subjectPutForm.Description}");

        departmentHandler.AuthToken = Request.Cookies["user_token"] as string;
        var subject = await departmentHandler.CreateSubject(1, subjectPutForm);

        return RedirectToPage("/Timetable/Subjects");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        personHandler.AuthToken = Request.Cookies["user_token"] as string;

        People = (await personHandler.GetPersons()).ToList();

        return Page();
    }
}