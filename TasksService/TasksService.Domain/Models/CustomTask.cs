﻿using System.Text.Json.Serialization;
using TasksService.Domain.Enums;

namespace TasksService.Domain.Models;

public class CustomTask
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Title { get; set; } 
    public string Description { get; set; }
    public Category Category { get; set; }
    public Priority Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MinutesBeforeDeadline { get; set; }
    
    [JsonIgnore]
    public ICollection<Tag> TaskTags { get; set; } = new List<Tag>();
    [JsonIgnore]
    public ICollection<Comment> TaskComments { get; set; } = new List<Comment>();
}