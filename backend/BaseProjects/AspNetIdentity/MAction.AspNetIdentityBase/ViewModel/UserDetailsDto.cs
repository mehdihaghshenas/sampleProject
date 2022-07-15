namespace MAction.AspNetIdentity.Base.ViewModel;

public class UserDetailsDto
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public object UserId { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreationTime { get; set; }
    public string Package { get; set; }
    public DateTime? PackageActivation { get; set; }
    public DateTime? PackageExpiration { get; set; }
}