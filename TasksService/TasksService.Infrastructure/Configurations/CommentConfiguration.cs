using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasksService.Domain.Models;

namespace TasksService.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(tag => tag.Id);
        builder.Property(tag => tag.Content).IsRequired().HasMaxLength(1000);
    }
}