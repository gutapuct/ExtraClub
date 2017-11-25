using System;
using System.Data.Objects;
using System.Web.Mvc;
using System.Linq;
using TonusClub.Entities;

namespace TonusClub.CustomerPortal.Controllers
{
    public class TreatmentEventsController : Controller
    {
        TonusEntities _context = new TonusEntities();

        public ActionResult Upcoming()
        {
            var uid = Guid.Parse(User.Identity.Name);
            var treatments = _context.TreatmentEvents
                .Where(i => i.CustomerId == uid && i.VisitDate >= DateTime.Today && i.VisitStatus == 0)
                .OrderBy(i => i.VisitDate)
                .GroupBy(i => EntityFunctions.TruncateTime(i.VisitDate).Value).ToArray();
            return View(treatments);
        }
    }
}