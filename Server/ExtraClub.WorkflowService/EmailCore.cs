using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Entities;
using ExtraClub.ServerCore;

namespace ExtraClub.WorkflowService
{
    public class EmailCore
    {
        public EmailCore()
        {

        }

        public void Send(ExtraClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            using (var context = new ExtraEntities())
            {
                var emails = context.EmailItems.Where(i => DateTime.Now >= i.SendFrom && !i.SentOn.HasValue && String.IsNullOrEmpty(i.LastErrorMessage)).ToList();
                foreach (var email in emails)
                {
                    if (cancelInfo.Cancel)
                    {
                        return;
                    }
                    try
                    {
                        if (!WorkflowCore.EmailValidate(email.Address))
                        {
                            throw new FormatException();
                        }

                        if (DateTime.Today > email.SendFrom.AddDays(1).Date)
                        {
                            throw new TimeoutException();
                        }

                        NotificationCore.SendMessage(email.Address, email.Subject, email.Message);
                        email.SentOn = DateTime.Now;
                    }
                    catch (FormatException)
                    {
                        email.SentOn = new DateTime(2001, 1, 1);
                        email.LastErrorMessage = "Форма указанной строки не годится для адреса электронной почты";
                    }
                    catch (TimeoutException)
                    {
                        email.SentOn = new DateTime(2002, 1, 1);
                        email.LastErrorMessage = "Сообщение не было отправлено вовремя. Обработано " + DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("5.1.1"))
                        {
                            email.SentOn = new DateTime(2000, 1, 1);
                            email.LastErrorMessage = "5.1.1";
                        }
                        else
                        {
                            email.SentOn = new DateTime(1999, 1, 1);
                            email.LastErrorMessage = ex.Message;
                        }
                    }
                    context.SaveChanges();

                }
            }
        }
    }
}
