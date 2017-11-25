using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using TonusClub.Entities;
using System.Linq;
using TonusClub.ServiceModel;

namespace TonusClub.CustomerPortal.Controllers
{
    public class TicketController : Controller
    {
        readonly TonusEntities _context = new TonusEntities();

        public ActionResult Details(Guid id)
        {
            var uid = Guid.Parse(User.Identity.Name);
            var ticket = _context.Tickets.Include("TicketType").FirstOrDefault(i => i.Id == id && i.CustomerId == uid);
            if (ticket == null)
            {
                return RedirectToAction("Tickets", "Home");
            }
            ticket.InitDetails();
            return View(ticket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="kind">
        /// 0 = Основные единицы
        /// 1 = Гостевые единицы
        /// </param>
        /// <returns></returns>
        public ActionResult Charges(Guid id, int kind)
        {
            ViewBag.Kind = kind;

            return View(_context.Tickets.Include("UnitCharges").Single(i => i.Id == id));
        }

        public ActionResult SolCharges(Guid id)
        {
            return View(_context.Tickets.Include("MinutesCharges").Single(i => i.Id == id));
        }

        public ActionResult Pay(Guid id)
        {
            var ticket = _context.Tickets.Single(i => i.Id == id);
            ticket.InitDetails();

            var tp = ticket.TicketPayments.ToList().FirstOrDefault(i => i.RequestedAmount > 0 && i.PaymentDate.Date == DateTime.Today);

            if (tp == null)
            {
                tp = new TicketPayment
                {
                    Amount = 0,
                    AuthorId = _context.Users.First(i => i.CompanyId == ticket.CompanyId).UserId,
                    CompanyId = ticket.CompanyId,
                    Id = Guid.NewGuid(),
                    PaymentDate = DateTime.Today,
                    RequestedAmount = (ticket.Cost - ticket.PaidAmount),
                    TicketId = ticket.Id,
                    TRoboNumber = _context.TicketPayments.Max(i => i.TRoboNumber ?? -1) + 1
                };
                _context.TicketPayments.AddObject(tp);
                _context.SaveChanges();
            }

            var sMrchLogin = "arcenciel";
            var sMrchPass1 = "Robokassa1";

            // номер заказа
            var nInvId = tp.TRoboNumber;

            // описание заказа
            var sDesc = String.Format("Оплата абонемента {0}", ticket.Number);

            // сумма заказа
            var sOutSum = tp.RequestedAmount.ToString("N2").Replace(",", ".").Replace(" ", "");

            // тип товара
            var sShpItem = "1";

            // предлагаемая валюта платежа
            var sIncCurrLabel = "";

            // язык
            var sCulture = "ru";

            // кодировка
            var sEncoding = "utf-8";

            // формирование подписи
            var sCrcBase = string.Format("{0}:{1}:{2}:{3}:Shp_item={4}:shpa={5}",
                                sMrchLogin, sOutSum, nInvId, sMrchPass1, sShpItem, tp.Id);

            var md5 = new MD5CryptoServiceProvider();
            var bSignature = md5.ComputeHash(Encoding.UTF8.GetBytes(sCrcBase));

            var sbSignature = new StringBuilder();
            foreach (var b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);

            var sCrc = sbSignature.ToString();

            var payUrl = "http://test.robokassa.ru/Index.aspx?" +
                                    "MrchLogin=" + sMrchLogin +
                                    "&OutSum=" + sOutSum +
                                    "&InvId=" + nInvId +
                                    "&Shp_item=" + sShpItem +
                                    "&SignatureValue=" + sCrc +
                                    "&Desc=" + sDesc +
                                    "&IncCurrLabel=" + sIncCurrLabel +
                                    "&Culture=" + sCulture +
                                    "&Encoding=" + sEncoding +
                                    "&shpa=" + tp.Id;
            return RedirectPermanent(payUrl);
        }
    }
}