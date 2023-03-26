using Microsoft.AspNetCore.Authorization;
using stag.Database;
using stag.Database.Models;

using System.Security.Claims;

public class SubjectSetGarantPermission : IAuthorizationRequirement {

}

public class SubjectRelationPermission : IAuthorizationRequirement {
    public RelationType RelationType { get; set; }

    public SubjectRelationPermission(RelationType relationType) {
        this.RelationType = relationType;
    }
}

public class SubjectAuthorizationHandler : IAuthorizationHandler
{
    public StagContext _stagContext { get; set; }
    
    public SubjectAuthorizationHandler(StagContext stagContext) {
        _stagContext = stagContext;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var httpContext = context.Resource as HttpContext;
        if(httpContext != null) {
            int subjectId = 0;
            string? subjectIdString = (string?)httpContext.Request.RouteValues["subjectId"];
            string? userId = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if(!string.IsNullOrEmpty(subjectIdString) &&
                int.TryParse(subjectIdString, out subjectId) &&
                userId != null) {
                var pendingRequirements = context.PendingRequirements.ToList();

                foreach(var requirement in pendingRequirements) {
                    if(requirement is SubjectSetGarantPermission) {
                        bool exists = _stagContext.Subjects.Join(_stagContext.DepartmentRelations,
                                                                    subject => subject.DepartmentId,
                                                                    relation => relation.DepartmentId,
                                                                    (subject, relation) => new {subject, relation})
                                                            .Any(x => x.relation.UserId == userId &&
                                                                    x.subject.SubjectId == subjectId);
                        if(exists) {
                            context.Succeed(requirement);
                        }
                    } else if(requirement is SubjectRelationPermission) {
                        bool exists = _stagContext.SubjectRelations.Any(x => x.SubjectId == subjectId &&
                                                                   x.UserId == userId &&
                                                                   x.RelationType == (requirement as SubjectRelationPermission).RelationType);
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