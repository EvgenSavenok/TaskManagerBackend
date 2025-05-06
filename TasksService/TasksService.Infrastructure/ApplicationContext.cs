using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        modelBuilder.Entity<CustomTask>()
            .HasMany(t => t.TaskTags)  
            .WithMany(t => t.TaskTags) 
            .UsingEntity<Dictionary<string, object>>(
                "CustomTaskTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<CustomTask>().WithMany().HasForeignKey("TaskId"));
    }
    
    public DbSet<CustomTask> Tasks { get; set; }
    
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<Comment> Comments { get; set; }
}
