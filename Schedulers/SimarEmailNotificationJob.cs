using Quartz;
using SimarAlertNotifier.Data;
using SimarAlertNotifier.Services;
using SimarAlertNotifier.Services.Mail;
using System.Net.Mail;
using System.Text;

namespace SimarAlertNotifier.Schedulers
{
    public class SimarEmailNotificationJob : IJob
    {
        private readonly ILogger<SimarEmailNotificationJob> _logger;
        private readonly IMailService _mailService;
        private readonly SimarDbContext _context;
        private readonly SimarAlertService _simarAlertService;

        public SimarEmailNotificationJob(
            ILogger<SimarEmailNotificationJob> logger, 
            IMailService mailService, 
            SimarDbContext simarDbContext,
            SimarAlertService simarAlertService)
        {
            _logger = logger;
            _mailService = mailService;
            _context = simarDbContext;
            _simarAlertService = simarAlertService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("SimarEmailNotificationJob is executing at {time}", DateTime.UtcNow);

            var mailFrom = Environment.GetEnvironmentVariable("MAIL_DEFAULT_FROM");
            var serverUrl = Environment.GetEnvironmentVariable("SITE_URL");

            if (string.IsNullOrEmpty(mailFrom))
            {
                _logger.LogError("MAIL_FROM environment variable is not set.");
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(serverUrl))
            {
                _logger.LogError("SITE_URL environment variable is not set.");
                return Task.CompletedTask;
            }

            // Get All Aviable Alerts
            var alerts = _simarAlertService.GetAlertsAsync().Result;

            if (alerts.Count == 0)
            {
                _logger.LogInformation("No alerts found.");
                return Task.CompletedTask;
            }

            // Get All Subscribers
            var subscribers = _context.Subscribers.ToList().Select(sb => sb.Email);

            if (subscribers.Count() == 0)
            {
                _logger.LogInformation("No subscribers found.");
                return Task.CompletedTask;
            }

            // Prepare Email Content
            var emailContent = new StringBuilder();
            emailContent.AppendLine("<b>Novos Alertas do Simar<b>: <br>");
            foreach (var alert in alerts)
            {
                emailContent.AppendLine($"- <span style=\"color: red;\">{alert.message}<span> <br>");
            }

            // add unsubscribe link
            emailContent.AppendLine($"<br>Para cancelar a inscrição, clique <a href=\"{serverUrl}/Subscriber/Create\">aqui</a>.");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(mailFrom);
            mail.Subject = "Novos Alertas do Simar";
            mail.Body = emailContent.ToString();
            mail.IsBodyHtml = true;

            foreach (var subscriber in subscribers)
            {
                mail.Bcc.Add(subscriber);
            }

            // Send Email
            _mailService.Send(mail);

            return Task.CompletedTask;
        }
    }
}
