using Infrastructure.EmailService;
using MAction.AspNetIdentity.Base;
using Infrastructure.EmailService.Models;

namespace MAction.SampleOnion.Infrastructure.EmailService;

public class UserEmailSender : IUserEmailSender
{
    private readonly IEmailSender _emailSender;

    public UserEmailSender(IEmailSender emailSender) => _emailSender = emailSender;

    public async Task SendEmailAsync(string toEmailAddress, string emailSubject, string verificationCode) => 
        await _emailSender.SendUserConfirmationMailAsync(
            new UserConfirmationMailModel()
            {
                VerificationCode = verificationCode,
                Subject = emailSubject,
                To = toEmailAddress,
                Bcc = toEmailAddress,
                Cc = toEmailAddress
            });
}
