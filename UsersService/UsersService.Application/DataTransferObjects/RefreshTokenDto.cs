namespace UsersService.Application.DataTransferObjects;

public record RefreshTokenDto
{
    public string AccessToken { get; set; }
}