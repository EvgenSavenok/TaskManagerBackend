namespace Application.DataTransferObjects.TagsDto;

public record TagDto
{
    public Guid Id { get; set; }
    public string TagName { get; set; } 
}