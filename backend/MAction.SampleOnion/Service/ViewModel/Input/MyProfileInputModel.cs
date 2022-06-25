using MAction.BaseClasses;
using FluentValidation;
using PhoneNumbers;
using System;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.SampleOnion.Service.ViewModel.Input;

public class MyProfileInputModel : BaseDTO<ApplicationUser, MyProfileInputModel>
{
    public string username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OrganizationName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class MyProfileValidator : AbstractValidator<MyProfileInputModel>
{
    public MyProfileValidator()
    {
        RuleFor(x => x.username).NotNull().WithMessage("NotNull");
        RuleFor(x => x.FirstName).NotNull().WithMessage("NotNull");
        RuleFor(x => x.LastName).NotNull().WithMessage("NotNull");
        RuleFor(x => x.Email).NotNull().WithMessage("NotNull").EmailAddress().WithMessage("Please enter the email format correctly");
        PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        RuleFor(x => x.PhoneNumber).NotNull().WithMessage("NotNull").Must(x =>
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