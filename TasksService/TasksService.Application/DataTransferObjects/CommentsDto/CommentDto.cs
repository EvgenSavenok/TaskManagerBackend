namespace Application.DataTransferObjects.CommentsDto;

public record CommentDto
{
    public Guid CommentId { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; }
}