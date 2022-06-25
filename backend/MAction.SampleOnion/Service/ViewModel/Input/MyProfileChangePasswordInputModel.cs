using MAction.BaseClasses;
using FluentValidation;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.SampleOnion.Service.ViewModel.Input;

public class MyProfileChangePasswordInputModel : BaseDTO<ApplicationUser, MyProfileChangePasswordInputModel>
{
    public string username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class MyProfileChangePasswordValidator : AbstractValidator<MyProfileChangePasswordInputModel>
{
    public MyProfileChangePasswordValidator()
    {
        RuleFor(x => x.username).NotNull().WithMessage("NotNull");
        RuleFor(x => x.Password).NotNull().WithMessage("NotNull");
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("MinimumLength6");
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("Confirm password must be same with password");
    }
}