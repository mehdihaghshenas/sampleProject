using System.ComponentModel.DataAnnotations;
using FluentValidation;
using PhoneNumbers;

namespace MAction.AspNetIdentity.Base.ViewModel;

public class RegisterViewModel
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public class RegisterAdminValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterAdminValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("NotEmpty").Must(x => x.Length <= 50);
            RuleFor(x => x.LastName).NotEmpty().WithMessage("NotEmpty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("NotEmpty").Must(x => x.Length is >= 6 and <= 50)
                .WithMessage("The Password must be at least 6 and at max 50 characters long.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("NotEmpty").Equal(x => x.Password)
                .WithMessage("The password and confirmation password do not match.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("NotEmpty").EmailAddress()
                .WithMessage("Please enter the email format correctly");
            var phoneUtil = PhoneNumberUtil.GetInstance();
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("NotEmpty").Must(x =>
            {
                if (x == null)
                    return true;
                try
                {
                    var q = phoneUtil.Parse(x, null);
                    return phoneUtil.IsValidNumber(q);
                }
                catch (Exception)
                {
                    return false;
                }
            }).WithMessage("Phone number invalid");
        }
    }
}
