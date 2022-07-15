namespace MAction.AspNetIdentity.Infrastructure.Models;

public class UserPoliciesInputModel
{
    public IEnumerable<string>? AddPolicies { get; set; }
    public IEnumerable<string>? RemovePolicies { get; set; }
    public string UserId { get; set; }

    public UserPoliciesInputModel()
    {
        UserId = "";
    }
}