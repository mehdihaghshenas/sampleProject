using System.ComponentModel.DataAnnotations;

namespace MAction.AspNetIdentity.Base.ViewModel;

public class RegisterViewModel
{
    [StringLength(50)]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(16)]
    public string PhoneNumber { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
