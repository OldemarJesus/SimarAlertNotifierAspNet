using Microsoft.AspNetCore.Mvc;
using SimarAlertNotifier.Models;
using SimarAlertNotifier.Models.Forms;
using SimarAlertNotifier.Services;
using SimarAlertNotifier.Services.Mail;
using System.Diagnostics;

namespace SimarAlertNotifier.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SimarAlertService _alertService;
        private readonly IMailService _mailService;

        public HomeController(ILogger<HomeController> logger, SimarAlertService alertService, IMailService mailService)
        {
            _logger = logger;
            _alertService = alertService;
            _mailService = mailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Alert()
        {
            // load alerts from external simar api
            List<Alert>? alerts = _alertService.GetAlertsAsync().Result;
            return View(alerts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
