using System.Net.Mail;

namespace SimarAlertNotifier.Services.Mail
{
    public interface IMailService
    {
        public SmtpClient Mail { get; }
        public void Send(MailMessage message);
        public void Send(string to, string subject, string message);
    }
}
