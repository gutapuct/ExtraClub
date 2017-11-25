using Sync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Web.Http;
using System.Web.Mvc;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace Sync.Controllers
{
    public class PublicApiController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetDivisionTreatments(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                return Json(context.Treatments.Where(d => d.DivisionId == divisionId)
                        .Where(t => t.IsActive)
                        .Select(t => t.TreatmentTypeId)
                        .Distinct()
                        .ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetClaims(string CompanyVopsName)
        {
            using (var context = new TonusEntities())
            {
                if (String.IsNullOrWhiteSpace(CompanyVopsName)) return Json(new { success = false, message = "Укажите пользователя" }, JsonRequestBehavior.AllowGet);

                var companyIds = context.Companies.Where(i => i.CompanyVopsEmail.Trim().ToLower() == CompanyVopsName.Trim().ToLower()).Select(i => i.CompanyId).ToArray();

                if (companyIds.Length == 0) return Json(new { success = false, message = "Учетная запись не привязана. Обратитесь в техническую поддержку." }, JsonRequestBehavior.AllowGet);
                if (companyIds.Length > 1) return Json(new { success = false, message = "Учетная запись привязана к нескольким компаниям. Обратитесь в техническую поддержку." }, JsonRequestBehavior.AllowGet);

                var companyId = companyIds[0];

                var claims = context.Claims.Where(i => i.CompanyId == companyId && i.ClaimTypeId != 3)
                    .Select(i => new
                    {
                        i.FtmId,
                        i.CreatedOn,
                        i.Subject,
                        i.StatusDescription,
                        i.StatusId,
                        i.FinishDate,
                        i.FinishedByName,
                        i.Message,
                        i.PrefFinishDate,
                        i.ContactInfo,
                        i.ContactEmail,
                        i.ContactPhone,
                    }).ToList();

                return Json(new {success = true, claims}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmClaim(int ftmId, int actualScore = 10)
        {
            using (var context = new TonusEntities())
            {
                var user = context.Users.Where(i => i.EmployeeId.HasValue).OrderBy(i => i.CreatedOn).Select(i => i.UserId).FirstOrDefault();
                var claim = context.Claims.FirstOrDefault(i => i.FtmId == ftmId);

                if (user != null && claim != null)
                {
                    claim.StatusDescription = "Закрыта";
                    claim.SubmitDate = DateTime.Now;
                    claim.SubmitUser = user;

                    var client = new Service();
                    client.PostClaimSubmit(claim.Id);
                    claim.StatusId = 6;
                    if (actualScore >= 0 && actualScore <= 10)
                    {
                        claim.ActualScore = actualScore;
                    }
                    
                    context.SaveChanges();
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReopenClaim(int ftmId, string message)
        {
            using (var context = new TonusEntities())
            {
                var claim = context.Claims.FirstOrDefault(i => i.FtmId == ftmId);

                if (claim != null)
                {
                    claim.FinishDescription = "Возобновление задачи: " + message;

                    var client = new Service();
                    client.PostClaimReopen(claim.Id, claim.FinishDescription);

                    claim.StatusId = 2;
                    claim.StatusDescription = "Запрос возобновлен в FTM";
                    claim.FinishedByFtmId = null;
                    claim.FinishedByName = null;
                    claim.FinishDate = null;
                    context.SaveChanges();
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SetupRating (Guid divisionId, int rating, Guid? entityId, string entityName = "")
        {
            using (var context = new TonusEntities())
            {
                if (!context.Divisions.Where(i => i.Id == divisionId).Any()) return RedirectToAction("SetupDivisionStar", "Home", new { rating = rating, message = "Клуб не найден. Обратитесь к администратору." });

                if (entityId.HasValue)
                {
                    var oldRaiting = context.DivisionStars.Where(i => i.EntityId == entityId).FirstOrDefault();
                    if (oldRaiting != null)
                    {
                        oldRaiting.Rating = rating;
                        context.SaveChanges();
                        return RedirectToAction("SetupDivisionStar", "Home", new { rating = rating, isChange = true });
                    }

                    var factory = new ChannelFactory<ClaimServiceContract.IClaimService>("ClaimServiceEndpoint");
                    var channel = factory.CreateChannel();
                    using (channel as IDisposable)
                    {
                        var ratingString = new RatingModel(rating).Rating;
                        channel.AddCustomerCommentAsync(
                            divisionId.ToString(),
                            String.Format("Поставлена новая звезда на сайте: {0}", ratingString)
                        );
                    }
                }

                context.DivisionStars.AddObject(new DivisionStar
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    DivisionId = divisionId,
                    EntityId = entityId.HasValue? (Guid)entityId : Guid.Empty,
                    Rating = rating,
                    EntityName = entityName,
                });
                context.SaveChanges();

                return RedirectToAction("SetupDivisionStar", "Home", new { rating = rating });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetDivisionStars()
        {
            using (var context = new TonusEntities())
            {
                var divisionStars = new List<DivisionStarsModel>();
                var divisions = context.Divisions.Select(i => i.Id).ToList();
                foreach (var div in divisions)
                {
                    divisionStars.Add(new DivisionStarsModel(div));
                }
                
                return Json(new
                {
                    success = true,
                    divisionStars = divisionStars.Select(i => new
                    {
                        AvgStars = Decimal.Round(i.AvgStars, 1),
                        DivisionId = i.DivisionId
                    }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddClaim (VopsClaim VopsClaim)
        {
            if (!new int[] { 0, 1, 2 }.Contains(VopsClaim.ClaimTypeId)) return Json(new { success = true, message = "ClaimTypeId is not valid" });
            VopsClaim.AddClaim();
            return Json(new { success = true });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetClubsByCompanyVopsName(string companyVopsName)
        {
            using (var context = new TonusEntities())
            {
                var companyId = context.Companies.Where(i => i.CompanyVopsEmail == companyVopsName).Select(i => i.CompanyId).FirstOrDefault();
                if (companyId == null)
                {
                    return Json(new { success = false, message = "Incorrect companyVopsName" });
                }

                var divisions = context.Divisions.Where(i => i.CompanyId == companyId).Select(i => new { divisionId = i.Id, name = i.Name }).ToList();
                return Json(new { success = true, Divisions = divisions }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetTreatments(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                if (!context.Divisions.Where(i => i.Id == divisionId).Any())
                {
                    return Json(new { success = false, message = "Incorrect divisionid" });
                }

                var treatments = context.Treatments
                    .Where(i => i.DivisionId == divisionId && i.IsActive)
                    .Select(i => new
                    {
                        id = i.Id,
                        name = ((i.TreatmentType.Name.Length > 25) ? i.TreatmentType.Name.Substring(0, 24) + "..." : i.TreatmentType.Name) + " - " + (i.Tag ?? ""),
                        dateDelivety = i.Delivery,
                        serialNumber = i.SerialNumber,
                        modelName = i.ModelName
                    }).OrderBy(i => i.name).ToList();
                return Json(new { success = true, Treatments = treatments }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDataForDivisionRating()
        {
            if (!Request.UserHostAddress.Contains("185.124.190.162") && !Request.UserHostAddress.Contains("185.124.190.163") && !Request.UserHostAddress.Contains("185.124.190.164"))
            {
                return Content("Incorrect IP: " + Request.UserHostAddress);
            }
            var model = new DivisionRatingModel();
            var result = model.GetData();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public void ExternalLog(Guid companyId, Guid? userId, string message)
        {
            using (var context = new SyncMetadataEntities())
            {
                context.ExternalLogs.AddObject(new ExternalLog
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    CompanyId = companyId,
                    UserId = userId,
                    Message = message
                });
                context.SaveChanges();
            }
        }
    }
}