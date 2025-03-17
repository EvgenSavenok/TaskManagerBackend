using TasksService.Domain.Enums;

namespace Application.RequestFeatures;

public class TaskParameters : RequestParameters
{
    public DateTime Deadline { get; set; } 
}