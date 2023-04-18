using Microsoft.AspNetCore.Mvc;
using Model.Request;
using Model.Response;
using stag.Database;
using stag.Database.Models;
using Stag.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Stag.Controllers;

/// <summary>
/// Handles subjects (courses)
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]/{subjectId}")]
public class SubjectController : ControllerBase
{
    private readonly SubjectService subjectService;
    private readonly StagContext context;
    
    public SubjectController(StagContext context)
    {
        subjectService = new SubjectService(context);
        this.context = context;
    }

    private string? GetToken() {
        if(Request.Headers.Authorization.Count > 0) {
            var splits = Request.Headers.Authorization[0]?.Split(' ');
            if(splits != null && splits.Length == 2) {
                return splits[1];
            }
        }
        return null;
    }

    [Authorize(Policy="IsGarant")]
    [HttpPost("Set")]
    public async Task<IActionResult> Set(int subjectId, [FromBody] SubjectSetRequest subject) 
    {
        try {
            await subjectService.Set(subjectId, subject);
            return Ok();
        } catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetTimetableEvents")]
    public async Task<IEnumerable<TimetableEventGetResponse>> GetSubjectTimetableEvents(int subjectId) {
        return await subjectService.GetSubjectTimetableEvents(subjectId);
    }

    [HttpGet("GetSubjectRelations")]
    public async Task<IEnumerable<SubjectRelationGetResponse>> GetSubjectRelations(int subjectId) {
        return await subjectService.GetSubjectRelations(subjectId);
    }

    [Authorize(Policy="IsLecturer")]
    [HttpPut("CreateLectureTimetable")]
    public async Task<IActionResult> CreateLectureTimetable(int subjectId) {
        try {
            string? userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        
            if(!string.IsNullOrEmpty(userId)) {
                var entity = await subjectService.CreateLectureTimetable(userId, subjectId);
                await context.SaveChangesAsync();
                return Created(nameof(CreateLectureTimetable), entity);
            } else {
                return Unauthorized();
            }
        } catch {
            return BadRequest();
        }
    }

    [Authorize(Policy="IsPracticioner")]
    [HttpPut("CreatePracticeTimetable")]
    public async Task<IActionResult> CreatePracticeTimetable(int subjectId) {
        try {
            string? userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        
            if(!string.IsNullOrEmpty(userId)) {
                var entity = await subjectService.CreatePracticeTimetable(userId, subjectId);
                await context.SaveChangesAsync();
                return Created(nameof(CreateLectureTimetable), entity);
            } else {
                return Unauthorized();
            }
        } catch {
            return BadRequest();
        }
    }

    [Authorize(Policy="IsTimetableEventOwner")]
    [HttpPost("TimetableEvent/{eventId}/CreateTimes")]
    public async Task<IActionResult> SetTimes(int subjectId, int eventId, [FromBody] IEnumerable<SubjectSetTimeRequest> times) {
        await subjectService.SetTimes(subjectId, eventId, times);
        await context.SaveChangesAsync();

        return Ok();
    }


    [Authorize(Policy="SetSubjectGarantPermission")]
    [HttpGet("SetGarant/{userId}")]
    public async Task<IActionResult> SetGarant(int subjectId, string userId) {
        await subjectService.SetGarant(subjectId, userId);
        await context.SaveChangesAsync();
        
        return Ok();
    }

    [Authorize(Policy="IsGarant")]
    [HttpGet("SetPracticioner/{userId}")]
    public async Task<IActionResult> AddPracticioner(int subjectId, string userId)
    {
        await subjectService.AddPracticioner(subjectId, userId);
        await context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy="IsGarant")]
    [HttpGet("SetTutor/{userId}")]
    public async Task<IActionResult> AddLecturer(int subjectId, string userId)
    {
        await subjectService.AddLecturer(subjectId, userId);
        await context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy="IsStudent")]
    [HttpPost("Enroll")]
    public async Task<IActionResult> Enroll(int subjectId)
    {
        return BadRequest();
    }
}