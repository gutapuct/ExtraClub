using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TonusClub.CustomerPortal.Business;
using TonusClub.Entities;
using TonusClub.ServiceModel;


namespace TonusClub.CustomerPortal.Controllers
{
    public class ShopController : Controller
    {
        readonly TonusEntities _context = new TonusEntities();
        public ActionResult Index()
        {
            var uid = Guid.Parse(User.Identity.Name);
            var user = _context.Customers.Single(i => i.Id == uid);
            var ticketTypes = _context.TicketTypesShops.Where(i => i.CompanyId == user.CompanyId).Select(i=>i.TicketType).ToList();
            return View(new ShopModel  { Division = _context.Divisions.Single(i => i.Id == user.ClubId), TicketTypes = ticketTypes });
        }

        public ActionResult Details(Guid id)
        {
            var ticketType = _context.TicketTypes.Single(i => i.Id == id);

            return View(ticketType);
        }

        public ActionResult Buy(Guid id)
        {
            var ticketType = _context.TicketTypes.Single(i => i.Id == id);

            return View(ticketType);
        }

        public ActionResult PaymentSale(Guid id)
        {
            var uid = Guid.Parse(User.Identity.Name);
            var tid = SaleBL.CreateTicket(uid, id);
            return RedirectToAction("Pay", "Ticket", new { id = tid });
        }

        public ActionResult NonPaymentSale(Guid id)
        {
            var uid = Guid.Parse(User.Identity.Name);
            var tid = SaleBL.CreateTicket(uid, id);
            return RedirectToAction("Details", "Ticket", new { id = tid });
        }
    }

    public class ShopModel
    {
        public Division Division { get; set; }
        public List<TicketType> TicketTypes { get; set; }
    }
}