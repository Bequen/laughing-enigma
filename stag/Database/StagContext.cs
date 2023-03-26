using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using stag.Controllers;
using stag.Database.Models;
using Stag.Database.Models;

namespace stag.Database;

public class StagContext : DbContext
{
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<SemesterEvent> SemesterEvents { get; set; }

    public DbSet<Subject> Subjects { get; set; }

    public DbSet<SubjectRelation> SubjectRelations { get; set; }
    public DbSet<TimetableEvent> TimetableEvents { get; set; }
    public DbSet<TimetableEventTime> TimetableEventTimes { get; set; }
    public DbSet<Person> Persons { get; set; }

    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentRelation> DepartmentRelations { get; set; }

    public StagContext() {

    }

    public StagContext(DbContextOptions<StagContext> options) : base(options)
    {
        
    }

#region PERMISSION_EXPERIMENT
    /// <summary>
    /// Try to use token to see if user has permissions to view subject
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public IQueryable<Subject> GetSubjects(string? token) {
        TokenService tokenService = new TokenService();
        var principal = tokenService.Validate(token);
        var id = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        return Subjects.Join(SubjectRelations,
            sub => sub.SubjectId,
            rel => rel.SubjectId,
            (sub, rel) => new { Subject = sub, SubjectRelation = rel })
        .Where(x => x.SubjectRelation.UserId == (id ?? String.Empty))
        .Select(x => x.Subject)
        .AsNoTracking();
    }

    /// <summary>
    /// Try to use token to see if user has permissions to view subject
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public IEnumerable<Subject> SetSubjects(string? token) {
        TokenService tokenService= new TokenService();
        var principal = tokenService.Validate(token);
        var id = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        return Subjects.Join(SubjectRelations,
            sub => sub.SubjectId,
            rel => rel.SubjectId,
            (sub, rel) => new { Subject = sub, SubjectRelation = rel })
        .Where(x => x.SubjectRelation.RelationType == RelationType.Garant &&
                    x.SubjectRelation.UserId == (id ?? String.Empty))
        .Select(x => x.Subject);
    }

    public async Task AddSubject(string? token, Subject subject) {
        TokenService tokenService= new TokenService();
        var principal = tokenService.Validate(token);
        var id = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        /* TODO: Permissions */
        Subjects.AddAsync(subject);
    }
#endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres");
        /* TODO: Move this to config file */
        optionsBuilder.UseNpgsql("Host=database;Port=5432;Database=postgres;Username=postgres");
    }

    /// <summary>
    /// Remaps tables to use snake_case for relation and column names
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DepartmentRelation>().HasKey(x => new { x.DepartmentId, x.UserId });

        /* By default Entity Framework is using same names, as DbSet property 
         * name, which by standard should be CamelCase, but the database is 
         * using snake_case. This is just remapping using Regex */
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            mutableEntityType.SetTableName(Regex.Replace(mutableEntityType.GetTableName(), @"(\B)([A-Z])", @"_$2").ToLower());
            foreach (var property in mutableEntityType.GetProperties())
            {
                /* From CamelCase to snake_case */
                property.SetColumnName(Regex.Replace(property.Name, @"(\B)([A-Z])", @"_$2").ToLower());
            }
        }
    }
}