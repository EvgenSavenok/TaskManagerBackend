using Application.DataTransferObjects.CommentsDto;
using Application.DataTransferObjects.TagsDto;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;

namespace Application.DataTransferObjects.TasksDto;

public record TaskDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } 
    public string Description { get; set; }
    public Category Category { get; set; }
    public Priority Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<TagDto> TaskTags { get; set; } = new();
    public List<CommentDto> TaskComments { get; set; } = new ();
}