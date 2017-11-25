using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sync.Controllers
{
    public class TestController : Controller
    {
        [AllowCrossSiteJsonAttribute]
        public JsonResult Index()
        {
            return Json(new
            {
                firstName = "Иван",
                lastName = "Иванов",
                address = new
                {
                    streetAddress = "Московское ш., 101, кв.101",
                    city = "Ленинград",
                    postalCode = "101101"
                },
                phoneNumbers = new string[] { "812 123-1234", "916 123-4567" }
            }
                , JsonRequestBehavior.AllowGet);
        }

    }

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(filterContext);
        }
    }
}
