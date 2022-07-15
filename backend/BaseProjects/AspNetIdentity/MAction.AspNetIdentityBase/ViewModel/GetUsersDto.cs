namespace MAction.AspNetIdentity.Base.ViewModel;

public class GetUsersDto
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public object UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string DomainStatus { get; set; }
    public string Package { get; set; }
    public DateTime CreationTime { get; set; }
}