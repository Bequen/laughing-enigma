
using Microsoft.AspNetCore.Mvc;
using stag.Database;
using Model.Response;
using Model.Request;
using Stag.Database.Models;
using System.Net;
using System.Security.Claims;

namespace Stag.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase {
    private readonly StagContext _context;

    public PersonController(StagContext context) {
        this._context = context;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] PersonAddRequest person) {
        _context.Persons.Add(new Person {
            UserId = person.UserId,
            FirstName = person.FirstName,
            LastName = person.LastName
        });

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("Get/{page?}/{contains?}")]
    public IEnumerable<PersonGetResponse> Filter(int? page, String? contains) {
        return _context.Persons.Where(x => (x.FirstName + " " + x.LastName).Contains(contains ?? ""))
                    .Skip((page ?? 0) * 10).Take(10)
                    .Select(x => new PersonGetResponse { 
                        PersonId = x.PersonId,
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    });
    }

    [HttpGet("GetSubjects")]
    public async Task<IEnumerable<SubjectGetResponse>> GetSubjects([FromQuery] SubjectGetRequest request) {
        string? userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if(userId == null) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return new List<SubjectGetResponse>();
        }
        
        var query = _context.Subjects
                        .Join(_context.SubjectRelations,
                              subject => subject.SubjectId,
                              relation => relation.SubjectId,
                              (subject, relation) => new {subject, relation})
                        .Where(x => x.relation.UserId == userId);

        if (request.Filter != null)
        {
            query = query.Where(x => x.subject.Name.Contains(request.Filter));
        }

        return query.OrderBy(x => x.subject.SubjectId).Take(request.Limit ?? 100)
                    .Select(x => new SubjectGetResponse() {
                        SubjectId = x.subject.SubjectId,
                        Name = x.subject.Name,
                        ShortName = x.subject.ShortName,
                        Description = x.subject.Description
                    }).ToList().GroupBy(x => x.SubjectId, (x, el) => el.First());
    }

    [HttpGet("GetTimetableTimes")]
    public async Task<IEnumerable<TimetableEventTimeResponse>> GetTimetableTimes([FromQuery] TimetableEventTimesRequest request) {
        string? userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if(userId == null) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return new List<TimetableEventTimeResponse>();
        }

        Console.WriteLine($"Getting times for user {userId} prd");

        return _context.TimetableEventTimes.Join(_context.TimetableEvents,
                                                time => time.TimetableEventId,
                                                ev => ev.TimetableEventId,
                                                (time, ev) => new {time, ev})
                                            .Join(_context.SubjectRelations,
                                                time => time.ev.SubjectId,
                                                rel => rel.SubjectId,
                                                (time, rel) => new {time.ev, time.time, rel})
                                            .Where(x => x.time.StartsAt >= request.From.ToUniversalTime() &&
                                                        x.time.EndsAt <= request.To.ToUniversalTime() &&
                                                        x.rel.UserId == userId)
                                            .Join(_context.Subjects,
                                                time => time.ev.SubjectId,
                                                sub => sub.SubjectId,
                                                (time, sub) => new TimetableEventTimeResponse() {
                                                    TimetableEventTimeId = time.time.TimetableEventTimeId,
                                                    Event = new TimetableEventGetResponse() {
                                                        TimetableEventId = time.ev.TimetableEventId,
                                                        SubjectId = time.ev.SubjectId,
                                                        EventType = (int)time.ev.EventType
                                                    },
                                                    Subject = new SubjectGetResponse() {
                                                        SubjectId = sub.SubjectId,
                                                        Name = sub.Name,
                                                        ShortName = sub.ShortName,
                                                        Description = sub.Description
                                                    },
                                                    StartsAt = time.time.StartsAt,
                                                    EndsAt = time.time.EndsAt
                                                }).ToList().GroupBy(x => x.TimetableEventTimeId, (x, el) => el.First());
    }
}