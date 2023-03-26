using Model.Request;
using stag.Database;
using stag.Database.Models;

namespace Stag.Controllers;

public class DepartmentService {
    private readonly StagContext _context;

    public DepartmentService() {
        _context = new StagContext();
    }

    public DepartmentService(StagContext context)
    {
        _context = context;
    }

    public async Task<Subject> CreateSubject(int departmentId, SubjectPutRequest request) {
        var result = await _context.Subjects.AddAsync(new Subject() {
            Name = request.Name,
            ShortName = request.ShortName,
            Description = request.Description,
            DepartmentId = departmentId
        });

        await _context.SaveChangesAsync();

        return result.Entity;
    }
}