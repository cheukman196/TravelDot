using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TravelGroupAssignment1.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _sendGridKey;
        public EmailSender(IConfiguration configuration)
        {
            _sendGridKey = configuration["SendGrid:ApiKey"];
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client=new SendGridClient(_sendGridKey);
            var from = new EmailAddress("audrey.tjandra@georgebrown.ca", subject);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            var response=await client.SendEmailAsync(msg);
            System.Diagnostics.Debug.WriteLine(response);
            //throw new NotImplementedException();
        }
    }
}
