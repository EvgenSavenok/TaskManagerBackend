using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain;

namespace UsersService.Infrastructure;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) 
    : IdentityDbContext<User, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        modelBuilder.Entity<User>();
    }
    
    public DbSet<User> Users { get; set; }
}
