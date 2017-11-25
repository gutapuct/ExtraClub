using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TonusClub.CustomerPortal.Business;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.CustomerPortal.Controllers
{

    [Authorize]
    [AllowCrossSiteJson]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string registerPhone)
        {
            if (!String.IsNullOrEmpty(registerPhone))
            {
                ViewBag.Phone = registerPhone;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Login(string phoneNumber, string password, bool rememberMe)
        {
            using (var context = new TonusEntities())
            {
                var users = context.Customers.Where(i => i.Phone2 == phoneNumber);
                if (users.Count() == 0)
                {
                    return Json(new { result = "notExist" });
                }
                if (users.Count() > 1)
                {
                    return Json(new { result = "contactSupport" });
                }

                var user = users.Single();

                if (UserHasPassword(context, user))
                {
                    if (ValidatePassword(context, user, password))
                    {
                        System.Web.Security.FormsAuthentication.SetAuthCookie(user.Id.ToString(), rememberMe);
                        Request.Cookies.Add(new HttpCookie("Name", user.FullName));
                        return Json(new { success = true, userName = user.FullName }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return Json(new { result = "invalidPassword", error = "Неверный пароль" }, JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    SetupPassword(context, user);
                    return Json(new { result = "canSend" });
                }
            }
        }

        private void SetupPassword(TonusEntities context, Customer customer)
        {
            var conn = (context.Connection);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            var password = "";
            var r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 8; i++)
            {
                password += r.Next(9);
            }

            password = "123";
            //SmsCore.SendSms(customer.Phone2, "Ваш пароль для входа на портал: " + password);

            context.ExecuteStoreCommand(String.Format("update customers set password = '{1}' where Id='{0}'",
                customer.Id, password));

        }

        public JsonResult GetCurrentuser()
        {
            using (var context = new TonusEntities())
            {
                var id = Guid.Parse(User.Identity.Name);
                var user = context.Customers.Single(i => i.Id == id);
                {
                    return Json(new { fullName = user.FullName, phone = user.Phone2, id = user.Id }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private bool UserHasPassword(TonusEntities context, Customer user)
        {
            return GetUserPassword(context, user.Id) != null;
        }

        private string GetUserPassword(TonusEntities context, Guid customerId)
        {
            var conn = (context.Connection);
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            return new SqlCommand { Connection = (context.Connection) as SqlConnection, CommandText = String.Format("Select password from Customers where Id='{0}'", customerId) }.ExecuteScalar() as string;
        }

        private bool ValidatePassword(TonusEntities context, Customer user, string password)
        {
            return password == GetUserPassword(context, user.Id);
        }

        [AllowAnonymous]
        public ActionResult Register(string phone)
        {
            ViewBag.Phone = phone;
            using (var context = new TonusEntities())
            {
                ViewBag.Divisions = context.Divisions.OrderBy(i => i.Company.CityName).ThenBy(i => i.Name).ToDictionary(i => i.Id, i => i.Company.CityName + ", " + i.Name);
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Register(string phone, string firstname, string lastname, Guid? divisionId)
        {
            if (!divisionId.HasValue)
            {
                return Json(new { error = "Необходимо указать клуб, в который Вы хотели бы посещать!" });
            }
            try
            {
                using (var context = new TonusEntities())
                {
                    var res = AccountBL.Register(context, phone, firstname, lastname, divisionId.Value);
                    SetupPassword(context, res);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult LogOff()
        {
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

        public ActionResult Details()
        {
            var context = new TonusEntities();
            var uid = Guid.Parse(User.Identity.Name);
            var user = context.Customers.Single(i => i.Id == uid);
            return View(user);
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
