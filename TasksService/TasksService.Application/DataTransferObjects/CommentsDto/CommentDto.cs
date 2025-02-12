namespace Application.DataTransferObjects.CommentsDto;

public record CommentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; }
}