using Application.RequestFeatures;
using TasksService.Domain.Models;

namespace Application.Specifications;

public class TaskSpecification : BaseSpecification<CustomTask>
{
    public TaskSpecification(TaskParameters taskParameters)
    {
        ApplyOrderBy(task => task.OrderByDescending(t => t.Deadline));
    }
}