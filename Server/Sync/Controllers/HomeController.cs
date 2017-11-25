using Sync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sync.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var uri = Request.Url;
            var host = uri.GetLeftPart(UriPartial.Authority);
            if(host.Contains("tonusclub"))
            {
                return RedirectPermanent("http://www.tonusclub.ru");
            }

            return RedirectToActionPermanent("Index", "RegionalServers");
        }

        public ActionResult About()
        {
            return View();
        }
        
        public ActionResult SetupDivisionStar(int rating, string message, bool isChange = false)
        {
            var model = new RatingModel(rating) { IsChange = isChange, Message = message };
            return View(model);
        }
    }
}
