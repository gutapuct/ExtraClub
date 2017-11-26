using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServiceModel;

namespace ExtraClub.WorkflowService
{
    class SmsBirthdayCore
    {
        ExtraEntities context = new ExtraEntities() { CommandTimeout = 600 };

        public void DoSmsGen(ExtraClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            using (var context = new ExtraEntities())
            {
                var data = context.sp_GetSmsBirthday().ToArray();
                var customerIds = data.Select(j => j.CustomerId).ToList();
                var customers = context.Customers.Where(i => customerIds.Contains(i.Id)).ToList();
                var users = context.Users.ToList();

                foreach (var item in data)
                {
                    var customer = customers.Where(i => i.Id == item.CustomerId).Single();

                    var createdBy = users.Where(i => i.CompanyId == customer.CompanyId).OrderByDescending(i => i.LastLoginDate).FirstOrDefault();
                    if (createdBy == null) continue;

                    // create bonus
                    context.BonusAccounts.AddObject(new BonusAccount
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.Now,
                        CustomerId = item.CustomerId,
                        Amount = 200,
                        Description = "Начисление бонусов в честь Дня рождения клиента",
                        CompanyId = customer.CompanyId
                    });

                    //create sms
                    if (SmsCore.ValidatePhone(item.Phone2) && item.SendSms && item.SmsBirthday && item.SmsList)
                    {
                        context.SmsMessages.AddObject(new SmsMessage
                        {
                            CustomerId = item.CustomerId,
                            Id = Guid.NewGuid(),
                            Phone = SmsCore.FormatPhone(item.Phone2),
                            ToSendFrom = DateTime.Today.AddHours(12 - item.UtcCorr),
                            Text = "С Днем Рождения! Желаем легкости, стройности и красоты! Ваш ТОНУС-КЛУБ",
                            SkipCheck = true
                        });
                    }

                    //create email
                    if (customer.HasEmail.HasValue && customer.HasEmail.Value && WorkflowCore.EmailValidate(item.Email))
                    {
                        context.EmailItems.AddObject(new EmailItem
                        {
                            Id = Guid.NewGuid(),
                            Address = item.Email,
                            Subject = "С Днем Рождения!",
                            Message = "<p style='text-align:center'><img src='http://ExtraClub.ru/media/67532/e-mail_1_3.jpg'></p>",
                            CreatedOn = DateTime.Now,
                            SendFrom = DateTime.Today.AddHours(12 - item.UtcCorr)
                        });
                    }

                    context.SaveChanges();

                    if (cancelInfo.Cancel) return;
                }
            }
        }

        public void SubtractionBonuses(ExtraClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            using (var context = new ExtraEntities())
            {
                var threeMontsAgo = DateTime.Today.AddMonths(-3);
                var threeMontsAgoPlusDay = DateTime.Today.AddMonths(-3).AddDays(1).AddSeconds(-1);

                const string text = "Начисление бонусов в честь Дня рождения клиента";

                var bonuses = context.BonusAccounts.Where(i => i.CreatedOn >= threeMontsAgo).ToList();
                var customerIds = bonuses.Where(i => i.Description == text && i.CreatedOn <= threeMontsAgoPlusDay).Select(i => i.CustomerId).ToList();

                foreach (var customerId in customerIds)
                {
                    var bonus = bonuses.Where(i => i.Description != text && i.CustomerId == customerId).FirstOrDefault();
                    if (bonus == null)
                    {
                        var oldBonus = bonuses.Where(i => i.Description == text && i.CustomerId == customerId).FirstOrDefault();
                        context.BonusAccounts.AddObject(new BonusAccount
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = oldBonus.CreatedBy,
                            CreatedOn = DateTime.Now,
                            CustomerId = oldBonus.CustomerId,
                            Amount = -200,
                            Description = "Списание бонусов на день рождение за неиспользование",
                            CompanyId = oldBonus.CompanyId
                        });

                        context.SaveChanges();
                        if (cancelInfo.Cancel) return;
                    }
                }
            }
        }
    }
}
