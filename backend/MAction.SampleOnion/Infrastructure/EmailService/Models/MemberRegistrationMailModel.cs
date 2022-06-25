namespace Infrastructure.EmailService.Models
{
    public class MemberRegistrationMailModel : SendEmailModel {
        public string DomainUrl {get; set;}
        public string Username {get; set;}
        public string Password {get; set;}
        public string Email {get; set;}
        public string InternationalCode {get; set;}
    }
}