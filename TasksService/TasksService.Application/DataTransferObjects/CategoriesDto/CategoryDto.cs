namespace Application.DataTransferObjects.CategoriesDto;

public record CategoryDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
}
