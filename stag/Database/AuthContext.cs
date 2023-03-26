using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace stag.Database;

/// <summary>
/// Custom IdentityUserContext. Claim-only permissions.
/// </summary>
public class AuthContext : IdentityUserContext<IdentityUser>
{
    public DbSet<IdentityUser> Users { get; set; }
    
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres");
        optionsBuilder.UseNpgsql("Host=database;Port=5432;Database=postgres;Username=postgres");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            mutableEntityType.SetTableName(mutableEntityType.GetTableName().ToLower());
            foreach (var property in mutableEntityType.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());   
            }
        }
    }
}