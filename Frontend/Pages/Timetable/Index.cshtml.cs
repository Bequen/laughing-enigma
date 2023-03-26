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
        Times = await personHandler.GetTimetableTimes(DateTime.Now, DateTime.Now.AddDays(7));

        return Page();
    }
}