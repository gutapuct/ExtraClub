using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TonusClub.Entities;

namespace TonusClub.CustomerPortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new TonusEntities();
            var uid = Guid.Parse(User.Identity.Name);
            ViewBag.Tickets = context.Tickets.Count(i => i.CustomerId == uid);
            return View();
        }

        public ActionResult Tickets()
        {
            var context = new TonusEntities();
            var uid = Guid.Parse(User.Identity.Name);
            var tickets = context.Tickets.Where(i => i.CustomerId == uid).ToList();
            tickets.ForEach(i => i.InitDetails());
            var ticketsEx = tickets.OrderBy(i=>(byte)i.Status).GroupBy(i => i.StatusText).ToArray();
            return View(ticketsEx);
        }
    }
}
