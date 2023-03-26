using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using stag.Database;
using stag.Database.Models;

namespace Stag.Auth;

public abstract class DepartmentSubjectPermission : IAuthorizationRequirement {
    public virtual bool Handle() => false;
}

public class DepartmentSubjectCreatePermission : DepartmentSubjectPermission {

}

public class SubjectSetGarantPermission : IAuthorizationRequirement {

}

public class DepartmentAuthorizationHandler : IAuthorizationHandler
{
    public StagContext _stagContext { get; set; }
    
    public DepartmentAuthorizationHandler(StagContext stagContext) {
        _stagContext = stagContext;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var httpContext = context.Resource as HttpContext;
        if(httpContext != null) {
            int departmentId = 0;
            string? departmentIdString = (string?)httpContext.Request.RouteValues["departmentId"];
            string? userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if(!string.IsNullOrEmpty(departmentIdString) &&
                int.TryParse(departmentIdString, out departmentId) &&
                userId != null) {
                var pendingRequirements = context.PendingRequirements.ToList();

                foreach(var requirement in pendingRequirements) {
                    if(requirement is DepartmentSubjectCreatePermission) {
                        bool exists = _stagContext.DepartmentRelations.Any(x => x.UserId == userId &&
                                                                  x.DepartmentId == departmentId);

                        if(exists) {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}