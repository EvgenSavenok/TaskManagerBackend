namespace TasksService.Domain.Models;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<CustomTask> Tasks { get; set; } = new List<CustomTask>();
}
