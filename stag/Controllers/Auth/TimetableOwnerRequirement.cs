using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using stag.Database;

public class TimetableOwnerRequirement : IAuthorizationRequirement
{
    public TimetableOwnerRequirement() {
        
    }
}


public class TimetableOwnerHandler : AuthorizationHandler<TimetableOwnerRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TimetableOwnerRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;

        if(httpContext != null) {
            int eventId = 0;
            string? eventIdString = (string?)httpContext.Request.RouteValues["eventId"];
            Console.WriteLine($"EventId: {eventIdString}");
            if (eventIdString != null && int.TryParse(eventIdString, out eventId)) {
                string? userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if(userId != null) {
                    using(var db = new StagContext()) {
                        bool exists = db.TimetableEvents.Any(x => x.TimetableEventId == eventId &&
                                                                   x.OwnerId == userId);
                        if(exists) {
                            context.Succeed(requirement);
                        }
                    }
                } else {
                    Console.WriteLine("User not found");
                }
            }
        }

        return Task.CompletedTask;
    }
}