using Microsoft.AspNetCore.Mvc;
using SimarAlertNotifier.Data;
using SimarAlertNotifier.Models;
using SimarAlertNotifier.Models.Forms;

namespace SimarAlertNotifier.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly SimarDbContext _context;

        public SubscriberController(SimarDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SubscribeSimarAlertForm formData)
        {
            if (ModelState.IsValid)
            {
                Subscriber? existingSubscriber = null;

                // save subscriber to database
                formData.email = formData.email.ToLower();
                formData.operation = formData.operation.ToLower();

                switch(formData.operation)
                {
                    case "add":
                        // check if subscriber dont already exists
                        existingSubscriber = _context.Subscribers.FirstOrDefault(s => s.Email == formData.email);
                        if (existingSubscriber is not null)
                        {
                            ViewBag.Error = "A subscrição já existe.";
                            return View();
                        }

                        Subscriber subscriber = new Subscriber { Email = formData.email };
                        _context.Subscribers.Add(subscriber);
                        _context.SaveChanges();

                        ViewBag.Success = "A subscrição foi criada.";
                        return View();

                    case "del":
                        // check if subscriber already exists
                        existingSubscriber = _context.Subscribers.FirstOrDefault(s => s.Email == formData.email);
                        if (existingSubscriber is null)
                        {
                            ViewBag.Error = "A subscrição não existe";
                            return View();
                        }

                        _context.Remove(existingSubscriber);
                        _context.SaveChanges();

                        ViewBag.Success = "A subscrição foi cancelada.";
                        return View();
                    default:
                        ViewBag.Error = "Operação solicitada inválida.";
                        return View();
                }                
            }

            ViewBag.Error = "Alguma Coisa Correu Mal";
            return View();
        }
    }
}
