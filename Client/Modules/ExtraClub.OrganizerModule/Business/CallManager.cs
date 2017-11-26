using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using ExtraClub.OrganizerModule.Views.Calls.Windows;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.UIControls.Windows;
using ExtraClub.ServiceModel;
using System.Windows;
using ExtraClub.ServiceModel.Organizer;
using ExtraClub.OrganizerModule.ViewModels;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure;

namespace ExtraClub.OrganizerModule.Business
{
    public class CallManager
    {
        List<IncomingCallForm> forms;
        List<File> links;
        StringBuilder log = new StringBuilder();
        ClientContext context;

        DateTime Started = DateTime.Now;

        OrganizerLargeViewModel ModelContext;

        public CallManager(OrganizerLargeViewModel modelContext)
        {
            ModelContext = modelContext;
            context = ApplicationDispatcher.UnityContainer.Resolve<ClientContext>();
            forms = context.GetCallScrenarioForms();
            links = context.GetDivisionFiles();

            var wnd = ApplicationDispatcher.UnityContainer.Resolve<IncomingCallGeneralWindow>();
            wnd.Show();
            wnd.Closed += StartIncomingCall_Closed;
        }

        void StartIncomingCall_Closed(object sender, EventArgs args)
        {
            var wnd = sender as IncomingCallGeneralWindow;
            if (wnd.Result == IncomingResult.NewCustomer)
            {
                log.AppendLine("Нажата кнопка \"Новый клиент\"");
                StartNewCustomer();
            }
            else
                if (wnd.Result == IncomingResult.OldCustomer)
                {
                    log.AppendLine("Нажата кнопка \"Старый клиент\"");
                    StartOldCustomer();
                }
                else if (wnd.Result == IncomingResult.NewCustomerScrenario)
                {
                    log.AppendLine("Нажата кнопка \"Новый клиент - сценарий\"");
                    StartScrenario();
                }
                else if (wnd.Result == IncomingResult.NotACustomer)
                {
                    log.AppendLine("Нажата кнопка \"Не клиент\"");
                    StartNotACustomer();
                }
                else
                {
                    log.AppendLine("Нажата кнопка \"Отмена\"");
                    Save(CallResult.Cancelled, null);
                }
        }

        private void Save(CallResult result, Guid? customerId)
        {
            context.PostNewCall(log.ToString(), result, customerId, true, Started, null, null);
            if (ModelContext != null)
            {
                ModelContext.RefreshCalls();
            }
        }


        private void StartScrenario()
        {
            var start = forms.FirstOrDefault(i => i.IsStartForm);
            if (start == null)
            {
                ExtraWindow.Alert("Ошибка", "Сценарий не создан!");
            }
            var currentForm = start;
            if (currentForm == null) return;
            log.Append("Открытие формы ");
            log.AppendLine(currentForm.Header);
            var dlg = ApplicationDispatcher.UnityContainer.Resolve<CustomWindow>(new ResolverOverride[] {
                new ParameterOverride("form", currentForm),
                new ParameterOverride("links", links.Where(i=>i.Parameter==currentForm.Id).ToList()),
                new ParameterOverride("log", log)
            });
            dlg.Closed += Screnario_Closed;
            dlg.Show();
        }

        void Screnario_Closed(object sender, EventArgs args)
        {
            var dlg = sender as CustomWindow;
            if (dlg.Form.HasInputBox && !String.IsNullOrEmpty(dlg.TextResult))
            {
                log.AppendFormat("Введен текст: {0}\n", dlg.TextResult);
            }
            if (dlg.ButtonResult == null)
            {
                log.AppendLine("Нажата кнопка \"Отмена\"");
                Save(CallResult.Cancelled, null);
                return;
            };
            log.AppendFormat("Нажата кнопка {0}\n", dlg.ButtonResult.ButtonText);
            if (dlg.ButtonResult.ButtonAction == 1 && dlg.ButtonResult.Parameter.HasValue)
            {
                var currentForm = forms.SingleOrDefault(i => i.Id == dlg.ButtonResult.Parameter.Value);
                log.Append("Открытие формы ");
                log.AppendLine(currentForm.Header);
                var newdlg = ApplicationDispatcher.UnityContainer.Resolve<CustomWindow>(new ResolverOverride[] { new ParameterOverride("form", currentForm),
                new ParameterOverride("links",links.Where(i=>i.Parameter==currentForm.Id).ToList()),
                new ParameterOverride("log", log)});
                newdlg.Closed += Screnario_Closed;
                newdlg.Show();
            }
            else
            {
                log.AppendLine("Сценарий завершен");
                Save(CallResult.Screnario, null);
            }
        }

        private void StartNewCustomer()
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<IncomingNewCustomerWindow>();
            wnd.Show();
            wnd.Closed += NewCustomer_Closed;
        }

        void NewCustomer_Closed(object sender, EventArgs e)
        {
            var wnd = sender as IncomingNewCustomerWindow;
            if (wnd.Result == IncomingResult.OldCustomer)
            {
                log.AppendLine("Нажата кнопка \"Старый клиент\"");
                StartOldCustomer();
            }
            else
                if (wnd.Result == IncomingResult.NotACustomer)
            {
                log.AppendLine("Нажата кнопка \"Не клиент\"");
                StartNotACustomer();
            }
            else if (wnd.Result == IncomingResult.SaveClicked)
            {
                if (!String.IsNullOrEmpty(wnd.Comments))
                {
                    log.Append("Комментарии: ");
                    log.AppendLine(wnd.Comments);
                }
                log.AppendLine("Сохранение нового клиента");
                TestByPhone(() =>
                {
                    var customerId = context.PostCustomer(new Customer
                    {
                        AdvertTypeId = wnd.AdvertTypeId,
                        Email = wnd.Email,
                        FirstName = wnd.FirstName,
                        InvitorId = wnd.RecommendedById == Guid.Empty ? (Guid?)null : wnd.RecommendedById,
                        LastName = wnd.LastName,
                        SmsList = wnd.SmsList,
                        MiddleName = wnd.MiddleName,
                        AdvertComment = wnd.AdvertComment,
                        Phone2 = wnd.Phone,
                        SocialStatusId = wnd.SocialStatusId,
                        WorkPlace = wnd.WorkPlace,
                        WorkPhone = wnd.WorkPhone,
                        Position = wnd.Position,
                        Kids = wnd.Kids,
#if BEAUTINIKA
                        MaterialCoeff = 1,
#endif
                        ClubId = context.CurrentDivision.Id
                    });
                    context.PostCustomerStatuses(customerId, wnd.CurrentStatuses.Where(x => x.IsChecked).Select(x => x.Id).ToList());
                    log.AppendLine("ID клиента - " + customerId.ToString());
                    if (wnd.AddTreatments)
                    {
                        log.AppendLine("Запись на процедуры");
                        NavigationManager.MakeScheduleRequest(new Infrastructure.ScheduleRequestParams { Customer = context.GetCustomer(customerId) });
                    }
#if BEAUTINIKA
                    if (wnd.AddConsultation)
                    {
                        log.AppendLine("Запись на консультацию");
                        NavigationManager.MakeConsultationRequest(new Infrastructure.ScheduleRequestParams { Customer = context.GetCustomer(customerId) });
                    }
                    else
                    {
                        context.PostGroupCall(new Guid[] { customerId }, new Guid[0], "Необходимо предложить запись на консультацию в Студию", DateTime.Today.AddDays(7), DateTime.Today.AddDays(3));
                    }
                    if (wnd.TargetTypeId.HasValue)
                    {
                        context.PostCustomerTarget(new CustomerTarget
                        {
                            AuthorId = context.CurrentUser.UserId,
                            CompanyId = context.CurrentCompany.CompanyId,
                            CreatedOn = DateTime.Now,
                            CustomerId = customerId,
                            Id = Guid.NewGuid(),
                            TargetDate = DateTime.Now.AddMonths(1),
                            TargetText = "Цель на основании звонка",
                            TargetTypeId = wnd.TargetTypeId.Value
                        });
                    }
#endif
                    if (wnd.AddSolarium)
                    {
                        log.AppendLine("Запись в солярий");
                        NavigationManager.MakeNewSolariumVisitRequest(new Infrastructure.ParameterClasses.NewSolariumVisitParams { CustomerId = customerId });
                    }
                    log.AppendLine("Нажата кнопка сохранения");
                    Save(CallResult.NewCustomer, customerId);
                }, wnd.Phone);
            }
            else
            {
                log.AppendLine("Нажата кнопка \"Отмена\"");
                Save(CallResult.Cancelled, null);
            }
        }

        private void TestByPhone(Action onOkay, string phone)
        {
            var custId = context.GetCustomerIdByPhone(phone);
            if (custId != Guid.Empty)
            {
                //ExtraWindow.Confirm(ExtraClub.UIControls.Localization.Resources.Warning,
                //    ExtraClub.UIControls.Localization.Resources.CustomerPhoneWarning, w =>
                //    {
                //        if (w.DialogResult ?? false)
                //        {
                //            onOkay();
                //        }
                //        else
                //        {
                //            NavigationManager.MakeClientRequest(custId);
                //        }
                //    });
                ExtraWindow.Alert("Предупреждение", "Клиент с таким номером телефона уже существует! Перед Вами раннее созданная карточка этого клиента!");
                NavigationManager.MakeClientRequest(custId);
            }
            else
            {
                onOkay();
            }

        }

        private void StartOldCustomer()
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<IncomingOldCustomerWindow>();
            wnd.Show();
            wnd.Closed += OldCustomer_Closed;
        }

        void OldCustomer_Closed(object sender, EventArgs e)
        {
            var wnd = sender as IncomingOldCustomerWindow;
            if (wnd.Result == IncomingResult.NewCustomer)
            {
                log.AppendLine("Нажата кнопка \"Новый клиент\"");
                StartNewCustomer();
            }
            else
                if (wnd.Result == IncomingResult.NotACustomer)
                {
                    log.AppendLine("Нажата кнопка \"Не клиент\"");
                    StartNotACustomer();
                }
                else if (wnd.Result == IncomingResult.SaveClicked)
                {
                    if (!String.IsNullOrEmpty(wnd.Comments))
                    {
                        log.Append("Комментарии: ");
                        log.AppendLine(wnd.Comments);
                    }
                    log.AppendLine("Нажата кнопка сохранения");
                    Save(CallResult.OldCustomer, wnd.Customer == null ? (Guid?)null : wnd.Customer.Id);
                }
                else if (wnd.Result == IncomingResult.NewCustomerScrenario)
                {
                    log.AppendLine("Нажата кнопка \"Новый клиент - сценарий\"");
                    StartScrenario();
                }
                else
                {
                    log.AppendLine("Нажата кнопка \"Отмена\"");
                    Save(CallResult.Cancelled, null);
                }
        }

        private void StartNotACustomer()
        {
            var wnd = ApplicationDispatcher.UnityContainer.Resolve<IncomingNotACustomerWindow>();
            wnd.Show();
            wnd.Closed += NotACustomer_Closed;
        }

        void NotACustomer_Closed(object sender, EventArgs e)
        {
            var wnd = sender as IncomingNotACustomerWindow;
            if (wnd.Result == IncomingResult.NewCustomer)
            {
                log.AppendLine("Нажата кнопка \"Новый клиент\"");
                StartNewCustomer();
            }
            else
                if (wnd.Result == IncomingResult.OldCustomer)
                {
                    log.AppendLine("Нажата кнопка \"Старый клиент\"");
                    StartOldCustomer();
                }
                else if (wnd.Result == IncomingResult.SaveClicked)
                {
                    if (wnd.MakeTask)
                    {
                        log.AppendLine("Запрошена постановка задачи");
                        context.PostNewTask(GetNonClientManagers(), "Входящий звонок - не клиент", wnd.Comments, DateTime.Now.AddHours(1), 2);
                    }
                    if (!String.IsNullOrEmpty(wnd.Comments))
                    {
                        log.Append("Комментарии: ");
                        log.AppendLine(wnd.Comments);
                    }
                    log.AppendLine("Нажата кнопка сохранения");
                    Save(CallResult.NotACustomer, null);
                }
                else if (wnd.Result == IncomingResult.NewCustomerScrenario)
                {
                    log.AppendLine("Нажата кнопка \"Новый клиент - сценарий\"");
                    StartScrenario();
                }
                else
                {
                    log.AppendLine("Нажата кнопка \"Отмена\"");
                    Save(CallResult.Cancelled, null);
                }
        }

        private Guid[] GetNonClientManagers()
        {
            return context.GetEmployeeIdsWithPermission("NonClientTask").ToArray();
        }
    }
}
