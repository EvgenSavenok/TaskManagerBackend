namespace UsersService.Application.DataTransferObjects;

public class UserForRegistrationDto
{
    public enum UserRole
    {
        User,
        Administrator
    }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public UserRole Role { get; set; }
}
