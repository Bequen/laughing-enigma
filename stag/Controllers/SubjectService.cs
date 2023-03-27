using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Request;
using stag.Database;
using stag.Database.Models;
using Stag.Database.Models;

namespace Stag.Controllers;

public class SubjectService {
    private readonly StagContext _context;

    public SubjectService() {
        _context = new StagContext();
    }

    public SubjectService(StagContext context) {
        this._context = context;
    }

    public async Task Set(int subjectId, SubjectSetRequest subject) {
        var row = _context.Subjects.FirstOrDefault(x => x.SubjectId == subjectId);
        if(row != null) {
            row.Description = subject.Description;
            row.Name = subject.Name;
            row.ShortName = subject.ShortName;
            // await _context.SaveChangesAsync();
        } else {
            throw new Exception("No such subject");
        }
    }

    public async Task<TimetableEvent> CreateLectureTimetable(string userId, int subjectId) {
        if(!string.IsNullOrEmpty(userId)) {
            var result = _context.TimetableEvents.Add(new TimetableEvent() {
                SubjectId = subjectId,
                EventType = TimetableEventType.Lecture,
                OwnerId = userId
            });

            // await _context.SaveChangesAsync();

            return result.Entity;
        } else {
            throw new Exception("User not found");
        }
    }

    public async Task<TimetableEvent> CreatePracticeTimetable(string userId, int subjectId) {
        if(!string.IsNullOrEmpty(userId)) {
            var result = _context.TimetableEvents.Add(new TimetableEvent() {
                SubjectId = subjectId,
                EventType = TimetableEventType.Practice,
                OwnerId = userId
            });

            // await _context.SaveChangesAsync();

            return result.Entity;
        } else {
            throw new Exception("User not found");
        }
    }

    public async Task SetGarant(int subjectId, string userId) {
        await _context.SubjectRelations.AddAsync(new SubjectRelation()
        {
            RelationType = RelationType.Garant,
            SubjectId = subjectId,
            UserId = userId
        });
    }

    public async Task SetLecturer(int subjectId, string userId) {
        await _context.SubjectRelations.AddAsync(new SubjectRelation()
        {
            RelationType = RelationType.Lecturer,
            SubjectId = subjectId,
            UserId = userId
        });
    }

    public async Task SetPracticioner(int subjectId, string userId) {
        await _context.SubjectRelations.AddAsync(new SubjectRelation()
        {
            RelationType = RelationType.Practicioner,
            SubjectId = subjectId,
            UserId = userId
        });
    }

    private async Task<List<TimetableEventTime>> CreateTimesWithFrequency(int eventId, SubjectSetTimeRequest time, int jumpDays) {
        var entities = new List<TimetableEventTime>();
        var semester = _context.Semesters.Include(x => x.Events)
                            .Where(x => x.StartsAt <= time.StartsAt.ToUniversalTime() &&
                                        x.EndsAt >= time.EndsAt.ToUniversalTime())
                            .FirstOrDefault();

        if(semester == null) {
            throw new Exception("Failed to find semester for subject. Please insert a semester to be able to insert timetable with frequency.");
        }

        while(time.StartsAt <= semester.EndsAt) {
            entities.Add(new TimetableEventTime {
                StartsAt = time.StartsAt.ToUniversalTime(),
                EndsAt = time.EndsAt.ToUniversalTime(),
                TimetableEventId = eventId
            });

            time.StartsAt = time.StartsAt.AddDays(jumpDays);
            time.EndsAt = time.EndsAt.AddDays(jumpDays);
        }

        return entities;
    }

    public async Task SetTimes(int subjectId, int eventId, IEnumerable<SubjectSetTimeRequest> times) {
        var entities = new List<TimetableEventTime>();
        
        foreach(var time in times) 
        {
            switch(time.Frequence) {
                /* Custom */
                case 0:
                    entities.Add(new TimetableEventTime {
                        StartsAt = time.StartsAt.ToUniversalTime(),
                        EndsAt = time.EndsAt.ToUniversalTime(),
                        TimetableEventId = eventId
                    });
                    break;
                /* All */
                case 1:
                    entities.AddRange(await CreateTimesWithFrequency(eventId, time, 7));
                    break;
                /* Even */
                case 2:
                    entities.AddRange(await CreateTimesWithFrequency(eventId, time, 14));
                    break;
            }
        }

        await _context.TimetableEventTimes.AddRangeAsync(entities);
    }

    public async Task<IEnumerable<TimetableEvent>> GetSubjectTimetableEvents(int subjectId) {
        return _context.TimetableEvents.Where(x => x.SubjectId == subjectId);
    }
}