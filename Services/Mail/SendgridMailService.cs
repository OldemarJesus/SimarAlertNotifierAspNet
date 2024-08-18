using System.Net;
using System.Net.Mail;

namespace SimarAlertNotifier.Services.Mail
{
    public class SendgridMailService : IMailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _defaultFromEmail = string.Empty;

        // intialize mail service
        public SendgridMailService()
        {
            var url = Environment.GetEnvironmentVariable("MAIL_URL");
            var port = Environment.GetEnvironmentVariable("MAIL_PORT");
            var username = Environment.GetEnvironmentVariable("MAIL_USERNAME");
            var password = Environment.GetEnvironmentVariable("MAIL_PASSWORD");
            var defaultFromEmail = Environment.GetEnvironmentVariable("MAIL_DEFAULT_FROM");

            if (username is null || url is null || defaultFromEmail is null || password is null || port is null)
            {
                throw new InvalidDataException("Missing Sendgrid Mail Url or Sendgrid Mail Username or Sendgrid Mail Password or Port or Default To Email from configuration");
            }

            var client = new SmtpClient()
            {
                Host = url,
                Port = int.Parse(port),
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
                UseDefaultCredentials = false
            };


            _smtpClient = client;
            _defaultFromEmail = defaultFromEmail;
        }

        public SmtpClient Mail => _smtpClient;

        public void Send(MailMessage message)
        {
            // send email
            _smtpClient.SendMailAsync(message);
        }

        public void Send(string to, string subject, string message)
        {
            var mailMessage = new MailMessage();
            // setting email from
            mailMessage.From = new MailAddress(_defaultFromEmail);
            // setting email to
            mailMessage.To.Add(new MailAddress(to));
            // setting email subject
            mailMessage.Subject = subject;
            // setting email message
            mailMessage.Body = message;

            // send email
            Send(mailMessage);
        }
    }
}
