using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationsService.Domain.Enums;

namespace NotificationsService.Domain.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } 
    
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid TaskId { get; set; } 
    
    
    public string Title { get; set; } 
    
    public Status Status { get; set; } 

    public DateTime CreatedAt { get; set; } 
    
    public DateTime Deadline { get; set; } 
    
    public int MinutesBeforeDeadline { get; set; }

    public DateTime ReminderTime => Deadline.AddMinutes(-MinutesBeforeDeadline);
}