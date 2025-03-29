namespace UsersService.Application.DataTransferObjects;

public record RefreshTokenRequest
{
    public string AccessToken { get; set; }
}