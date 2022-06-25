using System.ComponentModel.DataAnnotations;

namespace MAction.AspNetIdentity.Base.ViewModel;
public class LoginViewModel
{
    public string EmailorPhoneNumber { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}
public class LoginCacheDto
{
    public int UserId { get; set; }
    public string GeneratedCode { get; set; }
    public string PostedCode { get; set; }
}