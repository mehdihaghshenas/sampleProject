using System.Threading.Tasks;
using Infrastructure.EmailService.Models;

namespace Infrastructure.EmailService
{
    public interface IEmailSender
    {
        Task SendAsync(SendEmailModel sendEmailModel);

        Task SendUserConfirmationMailAsync(UserConfirmationMailModel userConfirmationMailModel);
        Task SendDomainRegistrationMailAsync(DomainRegistrationMailModel domainRegistrationMailModel);
        Task SendMemberRegistrationMailAsync(MemberRegistrationMailModel memberRegistrationMailModel);

    }
}
