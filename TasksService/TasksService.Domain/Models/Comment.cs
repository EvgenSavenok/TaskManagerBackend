namespace TasksService.Domain.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public CustomTask CustomTask { get; set; }
}