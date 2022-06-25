using System.Text.RegularExpressions;
using Infrastructure.EmailService.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailServerSetting _emailServerSetting;
        private readonly IConfiguration configuration;

        public EmailSender(IOptionsSnapshot<EmailServerSetting> emailServerSetting, IConfiguration configuration)
        {
            _emailServerSetting = emailServerSetting.Value;
            this.configuration = configuration;
        }
        public async Task SendAsync(SendEmailModel sendEmailModel)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailServerSetting.FromName, _emailServerSetting.FromAddress));

            message.To.Add(MailboxAddress.Parse(sendEmailModel.To));
            message.Cc.Add(MailboxAddress.Parse(sendEmailModel.Cc));
            message.Bcc.Add(MailboxAddress.Parse(sendEmailModel.Bcc));

            message.Subject = sendEmailModel.Subject;

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = sendEmailModel.Body;

            message.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailServerSetting.Host, _emailServerSetting.Port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(_emailServerSetting.UserName, _emailServerSetting.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }

        }

        public async Task SendUserConfirmationMailAsync(UserConfirmationMailModel userConfirmationMailModel)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailServerSetting.FromName, _emailServerSetting.FromAddress));

            message.To.Add(MailboxAddress.Parse(userConfirmationMailModel.To));
            message.Cc.Add(MailboxAddress.Parse(userConfirmationMailModel.Cc));
            message.Bcc.Add(MailboxAddress.Parse(userConfirmationMailModel.Bcc));

            var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            string FilePath = contentRoot + "/wwwroot/EmailService/Templates/UserConfirmMailTemplate.html";
            string EmailTemplateText = File.ReadAllText(FilePath);

            string subject = GetEmailSubject(EmailTemplateText);
            message.Subject = subject == String.Empty ? "Mehdi Haghsheans Authentication" : subject;

            EmailTemplateText = string.Format(EmailTemplateText, userConfirmationMailModel.VerificationCode);

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = EmailTemplateText;

            message.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailServerSetting.Host, _emailServerSetting.Port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(_emailServerSetting.UserName, _emailServerSetting.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        
        public async Task SendDomainRegistrationMailAsync(DomainRegistrationMailModel domainRegistrationMailModel)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailServerSetting.FromName, _emailServerSetting.FromAddress));

            message.To.Add(MailboxAddress.Parse(domainRegistrationMailModel.To));
            message.Cc.Add(MailboxAddress.Parse(domainRegistrationMailModel.Cc));
            message.Bcc.Add(MailboxAddress.Parse(domainRegistrationMailModel.Bcc));

            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\DomainRegistrationMailTemplate.html";
            string EmailTemplateText = File.ReadAllText(FilePath);

            string subject = GetEmailSubject(EmailTemplateText);
            message.Subject = subject == String.Empty ? "Mehdi Haghsheans Notification" : subject;

            EmailTemplateText = string.Format(EmailTemplateText, domainRegistrationMailModel.DomainUrl, domainRegistrationMailModel.ScriptCode);

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = EmailTemplateText;

            message.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailServerSetting.Host, _emailServerSetting.Port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(_emailServerSetting.UserName, _emailServerSetting.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendMemberRegistrationMailAsync(MemberRegistrationMailModel memberRegistrationMailModel)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailServerSetting.FromName, _emailServerSetting.FromAddress));

            message.To.Add(MailboxAddress.Parse(memberRegistrationMailModel.To));
            message.Cc.Add(MailboxAddress.Parse(memberRegistrationMailModel.Cc));
            message.Bcc.Add(MailboxAddress.Parse(memberRegistrationMailModel.Bcc));

            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\MemberRegistrationMailTemplate.html";
            string EmailTemplateText = File.ReadAllText(FilePath);

            string subject = GetEmailSubject(EmailTemplateText);
            message.Subject = subject == String.Empty ? "Mehdi Haghshenas Notification" : subject;

            EmailTemplateText = string.Format(EmailTemplateText, 
                                              memberRegistrationMailModel.DomainUrl, 
                                              memberRegistrationMailModel.Email,
                                              memberRegistrationMailModel.Username,
                                              memberRegistrationMailModel.Password,
                                              memberRegistrationMailModel.InternationalCode);

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = EmailTemplateText;

            message.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_emailServerSetting.Host, _emailServerSetting.Port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(_emailServerSetting.UserName, _emailServerSetting.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        private string GetEmailSubject(string file)
        {
            Match m = Regex.Match(file, @"(?i)<title>\s*(.+?)\s*</title>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

    }
}