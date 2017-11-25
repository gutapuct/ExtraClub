using System;
using System.Linq;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace Sync.Models
{
    public class VopsClaim
    {
        public int ClaimTypeId { get; set; }
        public string CompanyVopsEmail { get; set; }
        public string ContactInfo { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string PrefFinishDate { get; set; }
        public Guid? Eq_TreatmentId { get; set; }
        public string Eq_BuyDate { get; set; }
        public string Eq_Serial { get; set; }
        public string Eq_TechContact { get; set; }
        public string Eq_SerialGutwell { get; set; }
        public string Eq_Model { get; set; }
        public string Eq_ClubAddr { get; set; }
        public string Eq_PostAddr { get; set; }

        public void AddClaim ()
        {
            using (var context = new TonusEntities())
            {
                var companyid = context.Companies.Where(i => i.CompanyVopsEmail == CompanyVopsEmail).Select(i => i.CompanyId).FirstOrDefault();
                if (companyid != null)
                {
                    context.Claims.AddObject(new Claim
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = companyid,
                        ClaimTypeId = ClaimTypeId,
                        CreatedBy = Guid.Empty,
                        CreatedOn = DateTime.Now,
                        StatusDescription = "Ожидает отправки",
                        ContactInfo = ContactInfo,
                        ContactEmail = ContactEmail,
                        ContactPhone = ContactPhone,
                        Subject = Subject,
                        Message = Message,
                        PrefFinishDate = PrefFinishDate,
                        StatusId = 0,
                        Eq_BuyDate = Eq_BuyDate,
                        Eq_ClubAddr = Eq_ClubAddr,
                        Eq_Model = Eq_Model,
                        Eq_PostAddr = Eq_PostAddr,
                        Eq_Serial = Eq_Serial,
                        Eq_SerialGutwell = Eq_SerialGutwell,
                        Eq_TechContact = Eq_TechContact,
                        Eq_TreatmentId = Eq_TreatmentId
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}