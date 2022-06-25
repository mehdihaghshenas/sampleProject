namespace Infrastructure.EmailService.Models
{
    public class UserConfirmationMailModel : SendEmailModel {
        public string VerificationCode {get; set;}
    }
}