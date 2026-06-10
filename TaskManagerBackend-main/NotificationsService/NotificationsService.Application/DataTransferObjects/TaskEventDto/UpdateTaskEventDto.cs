namespace NotificationsService.Application.DataTransferObjects.TaskEventDto;

public record UpdateTaskEventDto
{
    public Guid TaskId { get; set; }
    
    
    public string Title { get; set; } 
    
    public DateTime Deadline { get; set; }
    
    public int MinutesBeforeDeadline { get; set; }
    
    public string UserTimeZone { get; set; }
    
    public string UserEmail { get; set; }
}