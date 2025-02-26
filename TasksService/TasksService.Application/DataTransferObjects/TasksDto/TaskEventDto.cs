namespace Application.DataTransferObjects.TasksDto;

public record TaskEventDto
{
    public Guid TaskId { get; set; }
    
    
    public string Title { get; set; } 
    
    public DateTime Deadline { get; set; }
    
    public int MinutesBeforeDeadline { get; set; }
    
    public string UserTimeZone { get; set; }
    
    public string UserEmail { get; set; }
}