namespace MAction.AspNetIdentity.Base;

public interface IUserEmailSender
{
    Task SendEmailAsync(string toEmailAddress, string emailSubject, string verificationCode);
}