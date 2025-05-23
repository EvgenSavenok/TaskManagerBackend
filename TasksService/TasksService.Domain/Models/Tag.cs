﻿namespace TasksService.Domain.Models;

public class Tag
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public ICollection<CustomTask> TaskTags { get; set; } = new List<CustomTask>();
}