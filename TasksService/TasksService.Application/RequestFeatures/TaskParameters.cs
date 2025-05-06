using TasksService.Domain.Enums;

namespace Application.RequestFeatures;

public class TaskParameters : RequestParameters
{
    public Guid UserId { get; set; }
}