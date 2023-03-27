using Frontend.Api.Stag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model.Response;

namespace Frontend.Pages.Timetable;

public class DayListNode {
    public TimetableEventTimeResponse time;
    public DayListNode? next;
}

public class Index : PageModel
{
    private PersonHandler personHandler = new PersonHandler();

    public IEnumerable<TimetableEventTimeResponse> Times { get; set;}

    public async Task<IActionResult> OnGetAsync()
    {
        personHandler.AuthToken = Request.Cookies["user_token"];
        try {
            Times = await personHandler.GetTimetableTimes(DateTime.Now, DateTime.Now.AddDays(7));
        } catch (HttpRequestException e) {
            if(e.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                return RedirectToPage("/Auth");
            }
        } catch(Exception e) {
            return Page();
        }

        return Page();
    }
}