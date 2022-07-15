namespace MAction.AspNetIdentity.Base.ViewModel;

public class RegisteredUserDto
{
    public string Token { get; set; }
    public HttpContextAnalysis IPAnalysis { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public object UserId { get; set; }
    public string Code { get; set; }
    public bool IsRequiresTwoFactor { get; set; }
    public string EmailStar { get; set; }
    public string PhoneStar { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}