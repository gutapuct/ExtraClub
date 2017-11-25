using ClaimServiceContract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace Flagmax.WorkflowService
{
    public class ClaimsCore
    {
        internal void Process()
        {
            ProcessClaims();
            UpdateClaims();
            ProcessEmailClaims();
        }

        private void UpdateClaims()
        {
            using(var context = new TonusEntities())
            {
                var date = DateTime.Today.AddMonths(-2);
                var claims = context.Claims.Where(i => i.FtmId.HasValue && i.CreatedOn > date && !i.FinishedByFtmId.HasValue).Where(i => i.ClaimTypeId != 3).ToArray();
                if(claims.Any())
                {
                    var cf = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");
                    var client = cf.CreateChannel();
                    foreach(var claim in claims)
                    {
                        var cInfo = client.GetClaimInfo(claim.FtmId.Value);
                        if(cInfo == null)
                        {
                            claim.FtmId = null;
                            claim.StatusId = -1;
                            claim.StatusDescription = "Заявка удалена из FTM";
                        }
                        else
                        {
                            if(cInfo.StatusId.HasValue && cInfo.StatusId > claim.StatusId)
                            {
                                claim.StatusId = cInfo.StatusId.Value;
                                claim.StatusDescription = cInfo.StatusDescription;
                            }
                            else
                            {
                                if(cInfo.StatusDescription != claim.StatusDescription)
                                {
                                    claim.StatusDescription = cInfo.StatusDescription;
                                }
                            }
                            if(cInfo.KindId != claim.ClaimTypeId && cInfo.KindId != 3)
                            {
                                claim.ClaimTypeId = cInfo.KindId;
                            }
                            if(cInfo.FinishDate != claim.FinishDate)
                            {
                                claim.FinishDate = cInfo.FinishDate;
                                claim.FinishedByName = cInfo.FinishedByName;
                                claim.FinishedByFtmId = cInfo.FinishedByFtmId;
                                claim.FinishDescription = cInfo.FinishDescription;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        private void ProcessClaims()
        {
            using(var context = new TonusEntities())
            {
                var claimsToCheck = context.Claims.Where(i => i.StatusId == 4 && i.FtmId.HasValue).ToArray();

                    var cf = new ChannelFactory<IClaimService>("ClaimServiceEndpoint");
                    var client = cf.CreateChannel();


                    foreach(var claim in claimsToCheck)
                    {
                        if(client.CheckClaimClosed(claim.FtmId.Value))
                        {
                            claim.StatusId = 6;
                        }
                    }
                context.SaveChanges();
            }
        }

        private void ProcessEmailClaims()
        {

            string[] receivers;
            receivers = ConfigurationManager.AppSettings.Get("ClaimEmailDestination").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            using(var context = new TonusEntities())
            {
                foreach(var claim in context.Claims.Where(i => i.ClaimTypeId == 3 && i.StatusId != 5 && i.StatusId != 6).ToArray())
                {
                    var str = CompileClaimText(claim);
                    foreach(var rec in receivers)
                    {
                        try
                        {
                            NotificationCore.SendMessage(rec, "Горячая линия основателей сети", str);
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                            return;
                        }
                    }
                    claim.StatusId = 5;
                    claim.StatusDescription = "Заявка отправлена";
                    context.SaveChanges();
                }
            }
        }

        private string CompileClaimText(Claim claim)
        {
            var context = new TonusEntities();
            var res = new StringBuilder();
            res.AppendLine("Заявка создана: " + claim.CreatedOn.ToString("dd.MM.yyyy H:mm"));
            res.AppendLine("Франчайзи: " + context.Companies.Single(i => i.CompanyId == claim.CompanyId).CompanyName);
            res.AppendLine("Пользователь: " + claim.ContactInfo + ", " + claim.ContactEmail + ", " + claim.ContactPhone);
            res.AppendLine("Тема: " + claim.Subject);
            res.AppendLine("Сообщение: " + claim.Message);

            return res.ToString();
        }

    }
}
