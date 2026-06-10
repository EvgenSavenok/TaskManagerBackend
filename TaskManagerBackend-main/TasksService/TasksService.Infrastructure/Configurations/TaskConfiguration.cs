using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<CustomTask>
{
    public void Configure(EntityTypeBuilder<CustomTask> builder)
    {
        builder.HasKey(task => task.Id);
        builder.Property(task => task.Title).IsRequired().HasMaxLength(100);
        builder.Property(task => task.Description).IsRequired().HasMaxLength(1000);
        builder.Property(task => task.Priority).IsRequired();
        builder.Property(task => task.CreatedAt).IsRequired();
        builder.Property(task => task.Deadline).IsRequired();
        builder.Property(task => task.Category).IsRequired();
    }
}