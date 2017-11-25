using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.CustomerPortal.Controllers
{
    public class PaymentController : Controller
    {
        [AllowAnonymous]
        public ActionResult Completed(string OutSum, string InvId, string SignatureValue, string shpa)
        {
            var sCrcBase = string.Format("{0}:{1}:{2}:Shp_item=1:shpa={3}",
                    OutSum, InvId, "Robokassa2", shpa);


            var md5 = new MD5CryptoServiceProvider();
            var bSignature = md5.ComputeHash(Encoding.UTF8.GetBytes(sCrcBase));

            var sbSignature = new StringBuilder();
            foreach (var b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);

            var sCrc = sbSignature.ToString();

            if (sCrc.ToLower() != SignatureValue.ToLower())
            {
                return Content("Incorrect CRC! " + SignatureValue);
            }

            var context = new TonusEntities();

            var tpId = Guid.Parse(shpa);

            var tp = context.TicketPayments.Single(i => i.Id == tpId);

            var amount = Decimal.Parse(OutSum.Replace(".", ","));

            tp.Amount = amount;
            tp.IsRoboCompleted = true;

            var bo = new BarOrder
            {
                AuthorId = context.Users.First(i => i.CompanyId == tp.CompanyId).UserId,
                CardPayment = tp.Amount,
                CustomerId = tp.Ticket.CustomerId,
                DivisionId = tp.Ticket.DivisionId,
                Id = Guid.NewGuid(),
                Kind1C = (int)Kinds1CEnum.Ticket,
                OrderNumber = 0,
                PaymentComments = tp.Id.ToString(),
                PaymentDate = DateTime.Now,
                PurchaseDate = DateTime.Now
            };

            context.BarOrders.AddObject(bo);

            context.SaveChanges();

            return Content("OK" + InvId);
        }

        public ActionResult Success(string OutSum, string InvId, string SignatureValue, string Culture, string shpa)
        {
            var sCrcBase = string.Format("{0}:{1}:{2}:Shp_item=1:shpa={3}",
                    OutSum, InvId, "Robokassa1", shpa);


            var md5 = new MD5CryptoServiceProvider();
            var bSignature = md5.ComputeHash(Encoding.UTF8.GetBytes(sCrcBase));

            var sbSignature = new StringBuilder();
            foreach (var b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);

            var sCrc = sbSignature.ToString();

            if (sCrc != SignatureValue)
            {
                return Content("Incorrect CRC! " + SignatureValue);
            }

            var context = new TonusEntities();

            var tpId = Guid.Parse(shpa);
            var tp = context.TicketPayments.Single(i => i.Id == tpId);

            tp.IsRoboCompleted = true;
            context.SaveChanges();

            return View(tp);
        }

        public ActionResult Fail(string OutSum, string InvId, string Culture, string shpa)
        {
            var context = new TonusEntities();

            var tpId = Guid.Parse(shpa);
            var tp = context.TicketPayments.Single(i => i.Id == tpId);

            return RedirectToActionPermanent("Details", "Ticket", new { id = tp.TicketId });
        }
    }
}
