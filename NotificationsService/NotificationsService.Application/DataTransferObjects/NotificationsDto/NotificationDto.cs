using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.DataTransferObjects.NotificationsDto;

public record NotificationDto
{
    public Guid Id { get; set; } 

    public Guid UserId { get; set; }
    
    public Guid TaskId { get; set; } 
    
    
    public string Title { get; set; } 
    
    public Status Status { get; set; } 

    public DateTime CreatedAt { get; set; } 
    
    public DateTime Deadline { get; set; }
    
    public int MinutesBeforeDeadline { get; set; }
}