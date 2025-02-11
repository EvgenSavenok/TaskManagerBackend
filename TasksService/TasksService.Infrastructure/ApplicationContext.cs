using Microsoft.EntityFrameworkCore;
using TasksService.Domain.Models;
using TasksService.Infrastructure.Configurations;

namespace TasksService.Infrastructure;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.ApplyConfiguration(new TaskConfiguration());

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
