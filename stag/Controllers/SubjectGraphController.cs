using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using stag.Database;
using stag.Database.Models;

namespace stag.Controllers;

public class SubjectGraphController : ObjectController
{
    private StagContext _dbContext;
    private IAuthorizationService _authService;
    
    /// <summary>
    /// Creates new subject graph controller
    /// </summary>
    /// <param name="dbContext">Database context to use</param>
    public SubjectGraphController(StagContext dbContext, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _authService = authorizationService;
        
        
    }
    
    /// <summary>
    /// Gets all the subjects that have relation with the user
    /// </summary>
    /// <param name="userId">User Id which the subjects have relation to</param>
    /// <returns>Returns enumerable list of subjects, that have a relation to user</returns>
    [QueryRoot("subjects")]
    public IEnumerable<Subject> RetrieveSubjects(string userId, string? search = null, RelationType? relationType = null)
    {
        var query = _dbContext.SubjectRelations
            .Where(x => x.UserId == userId &&
                        x.RelationType == (relationType ?? x.RelationType))
            .Join(_dbContext.Subjects, 
                x => x.SubjectId,
                x => x.SubjectId,
                (rel, sub) => sub);

        return !string.IsNullOrEmpty(search) ? query.Where(x => x.Name.Contains(search)) : query;
    }
}