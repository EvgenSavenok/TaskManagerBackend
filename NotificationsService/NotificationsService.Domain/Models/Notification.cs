using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    
    public DateTime CreatedAt { get; set; } 
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)] 
    public DateTime Deadline { get; set; } 
    
    public int MinutesBeforeDeadline { get; set; }

    public DateTime ReminderTime => Deadline.AddMinutes(-MinutesBeforeDeadline);
    
    // Need to get user email from the broker
    // TODO
    
    public string UserEmail { get; set; } = "eugen.savenok2@gmail.com";
    
    public string UserTimeZone { get; set; }
    
    public string HangfireJobId { get; set; }
}