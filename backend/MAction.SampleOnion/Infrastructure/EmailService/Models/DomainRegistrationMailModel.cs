namespace Infrastructure.EmailService.Models
{
    public class DomainRegistrationMailModel : SendEmailModel {
        public string DomainUrl {get; set;}
        public string ScriptCode {get; set;}
    }
}