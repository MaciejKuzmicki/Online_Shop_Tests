using SendGrid;
using SendGrid.Helpers.Mail;
using System.Drawing;

namespace Projekt
{
    public class EmailSender
    {
        public static async Task SendEmail(string email, string username, string subject, string message)
        {
            string apikey = "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";
            var client = new SendGridClient(apikey);
            var from = new EmailAddress("mailforproject9@gmail.com", "Project.com");
            var to = new EmailAddress(email, username);
            var text = message;
            var content = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, text, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
