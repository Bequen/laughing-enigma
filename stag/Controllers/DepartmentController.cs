using Microsoft.AspNetCore.Mvc;
using Model.Request;
using Model.Response;
using stag.Database;
using stag.Database.Models;
using Stag.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stag.Auth;

namespace Stag.Controllers;

/// <summary>
/// Handles subjects (courses)
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]/{departmentId}")]
public class DepartmentController : ControllerBase
{
    private readonly StagContext _context;
    private readonly DepartmentAuthorizationHandler _authService;
    
    public DepartmentController(StagContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "CreateDepartmentSubjectPermission")]
    [HttpPut("CreateSubject")]
    public async Task<IActionResult> CreateSubject(int departmentId, SubjectPutRequest request) {
        var result = await _context.Subjects.AddAsync(new Subject() {
            Name = request.Name,
            ShortName = request.ShortName,
            Description = request.Description,
            DepartmentId = departmentId
        });

        await _context.SaveChangesAsync();

        return Created(nameof(CreateSubject), result.Entity);
    }
}