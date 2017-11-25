using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using TonusClub.CashRegisterModule;
using TonusClub.Clients.Business;
using TonusClub.Clients.ViewModels;
using TonusClub.Clients.Views.ContainedControls;
using TonusClub.Clients.Views.Windows;
using TonusClub.Clients.Views.Windows.CustomerAndCards;
using TonusClub.Clients.Views.Windows.Tickets;
using TonusClub.Infrastructure;
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.UIControls;
using TonusClub.UIControls.Interfaces;
using TonusClub.UIControls.Windows;
using WindowBase = TonusClub.UIControls.WindowBase;

namespace TonusClub.Clients.Views
{
    public partial class ClientLargeView : ILargeSection
    {
        readonly ClientLargeViewModel _model;

        readonly IReportManager _repMan;

        public ClientLargeView(ClientLargeViewModel model, ISettingsManager setMan, IReportManager repMan)
        {
            NavigationManager.NewClientRequest += navMan_NewClientRequest;
            NavigationManager.CustomerTargetRequest += _navMan_CustomerTargetRequest;
            NavigationManager.TreatmentEventRequest += NavigationManager_TreatmentEventRequest;
            NavigationManager.TicketRequest += NavigationManager_TicketRequest;
            NavigationManager.CustomerCardRequest += NavigationManager_CustomerCardRequest;
            NavigationManager.GoodSalesRequest += NavigationManager_GoodSalesRequest;
            NavigationManager.NewTicketRequest += NavigationManager_NewTicketRequest;

            NavigationManager.ClientInRequest += NavigationManager_ClientInRequest;
            NavigationManager.ClientOutRequest += NavigationManager_ClientOutRequest;

            _repMan = repMan;

            DataContext = _model = model;

            InitializeComponent();
            setMan.RegisterGridView(this, CustomerCardsGrid);
            setMan.RegisterGridView(this, CustomerTicketsView);

#if BEAUTINIKA
                DiariesBar.Header = "Дневники красоты";
#endif
        }

        void NavigationManager_ClientOutRequest(object sender, ClientEventArgs e)
        {
            ProcessUserDialog<RegisterComeOut>(() =>
            {
                e.OkAction();
                NavigationManager.RefreshBar();
            }, new ParameterOverride("customer", new Customer { Id = e.ClientId }));
        }

        void NavigationManager_ClientInRequest(object sender, ClientEventArgs e)
        {
            ProcessUserDialog<RegisterComeIn>(() =>
            {
                e.OkAction();
                NavigationManager.RefreshBar();
            }, new ParameterOverride("customer", new Customer { Id = e.ClientId }));
        }

        void NavigationManager_NewTicketRequest(object sender, GuidEventArgs e)
        {
            ProcessUserDialog<NewTicketWindow>(() =>
                {
                    _model.SelectClient(e.Guid);
                    e.OnClose();
                }, new ParameterOverride("customer", _model.ClientContext.GetCustomer(e.Guid)));

        }

        void NavigationManager_GoodSalesRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    NavigationBar.SelectedItem = AnketBar;
                    AnketBar.IsExpanded = true;
                    AnketTabs.SelectedIndex = 3;
                    _model.SelectClient(_model.ClientContext.GetCustomerByGoodSale(e.Guid));
                    var sa = _model._SalesView.FirstOrDefault(i => i.Id == e.Guid);
                    if (sa != null)
                    {
                        _model.SalesView.MoveCurrentTo(sa);
                    }
                }));
            });
        }

        void NavigationManager_CustomerCardRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    NavigationBar.SelectedItem = TicketsBar;
                    TicketsBar.IsExpanded = true;
                    TicketsTabs.SelectedIndex = 0;
                    _model.SelectClient(_model.ClientContext.GetCustomerByCardId(e.Guid));
                    if (_model.CurrentCustomer != null)
                    {
                        var ca = _model.CurrentCustomer.SerializedCustomerCards.FirstOrDefault(i => i.Id == e.Guid);
                        if (ca != null)
                        {
                            CustomerCardsGrid.SelectedItems.Add(ca);
                        }
                    }
                }));
            });
        }

        void NavigationManager_TicketRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    NavigationBar.SelectedItem = TicketsBar;
                    TicketsBar.IsExpanded = true;
                    TicketsTabs.SelectedIndex = 1;
                    _model.SelectClient(_model.ClientContext.GetCustomerIdByTicketId(e.Guid));
                    if (_model.CurrentCustomer != null)
                    {
                        var ti = _model.Tickets.FirstOrDefault(i => i.Id == e.Guid);
                        if (ti != null)
                        {
                            CustomerTicketsView.SelectedItems.Add(ti);
                        }
                    }
                }));
            });
        }

        void NavigationManager_TreatmentEventRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    NavigationBar.SelectedItem = VisitsBar;
                    VisitsBar.IsExpanded = true;
                    _model.SelectClient(_model.ClientContext.GetCustomerIdByTreatmentEventId(e.Guid));
                    var ev = _model._events.FirstOrDefault(i => i.Id == e.Guid);
                    if (ev != null)
                    {
                        CustomerBookingsView.SelectedItems.Add(ev);
                    }
                }));
            });
        }

        void _navMan_CustomerTargetRequest(object sender, GuidEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new Action(() =>
                {
                    NavigationBar.SelectedItem = DiariesBar;
                    DiariesBar.IsExpanded = true;
                    DiariesControl.Tabs.SelectedItem = DiariesControl.TargetsTab;
                    _model.CustomerTargetRequest(e);
                }));
            });
        }

        void navMan_NewClientRequest(object sender, ClientEventArgs e)
        {
            ProcessUserDialog<NewCustomerWindow>(j =>
            {
                if (j.DialogResult ?? false)
                {
                    e.OnSuccess(j.GuidResult);
                }
            });
        }

        public override void SetState(object data)
        {
            base.SetState(data);
            _model.EnsureDataLoading();
            if (data is string)
            {
                //TODO: Search for client
            }
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer != null && _model.CurrentTicket != null)
            {
                Tickets.ProcessTicketPayment(res =>
                {
                    if (res) _model.SelectClient(_model.CurrentCustomer.Id);
                }, ApplicationDispatcher.UnityContainer.Resolve<CashRegisterManager>(), ClientContext,
                    _model.CurrentCustomer,
                    _model.CurrentTicket);
            }
        }

        private void CustomerSearchControl_SelectedClientChanged(object sender, GuidEventArgs e)
        {
            _model.SelectClient(e.Guid);
        }

        private void NewTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer == null) return;
            ProcessUserDialog<NewTicketWindow>(() => _model.SelectClient(_model.CurrentCustomer.Id), new ParameterOverride("customer", _model.CurrentCustomer));
        }

        private void FreezeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<TicketFreezeWindow>();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<TicketReturnWindow>();
        }

        private void RebillTicketButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<TicketRebillWindow>();
        }

        private void TicketChangeButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<TicketChangeWindow>();
        }

        private void UpgradeCustomerCardButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<UpgradeCustomerCardWindow>(() => _model.SelectClient(_model.CurrentCustomer.Id),
                new ParameterOverride("customer", _model.CurrentCustomer), new ParameterOverride("isLost", false));
        }

        private void LostCustomerCardButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<UpgradeCustomerCardWindow>(() => _model.SelectClient(_model.CurrentCustomer.Id),
                new ParameterOverride("customer", _model.CurrentCustomer), new ParameterOverride("isLost", true));
        }

        private void NewCustomerCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_model.CurrentCustomer.PasspNumber) || String.IsNullOrEmpty(_model.CurrentCustomer.PassportData) || !CheckPasspDate(_model.CurrentCustomer.PasspEmitDate))
            {
                TonusWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.UnableToRegisterCardMessage,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = Application.Current.MainWindow
                });
                return;
            }

            ProcessUserDialog<NewCustomerCard>(() => _model.SelectClient(_model.CurrentCustomer.Id),
            new ParameterOverride("customer", _model.CurrentCustomer));
        }

        private bool CheckPasspDate(DateTime? nullable)
        {
            if (Thread.CurrentThread.CurrentUICulture.IetfLanguageTag.ToLower() != "ru-ru") return true;
            return nullable.HasValue;
        }

        private void ProcessUserDialog<T>()
            where T : WindowBase
        {
            if (_model.CurrentCustomer != null && _model.CurrentTicket != null)
            {
                ProcessUserDialog<T>(() => _model.SelectClient(_model.CurrentCustomer.Id), new ParameterOverride("customer", _model.CurrentCustomer), new ParameterOverride("ticket", _model.CurrentTicket));
            }
        }

        private void RegisterComeInButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<RegisterComeIn>(() =>
                {
                    _model.SelectClient(_model.CurrentCustomer.Id);
                    NavigationManager.RefreshBar();
                }, new ParameterOverride("customer", _model.CurrentCustomer));
        }

        private void RegisterComeOutButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<RegisterComeOut>(() =>
            {
                _model.SelectClient(_model.CurrentCustomer.Id);
                NavigationManager.RefreshBar();
            }, new ParameterOverride("customer", _model.CurrentCustomer));
        }

        public object GetTransferDataForMinimize()
        {
            return null;
        }

        public object GetTransferDataForRestore()
        {
            return null;
        }

        private void ActivateTicketButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTicket == null) return;
            TonusWindow.Confirm(UIControls.Localization.Resources.TicketActivation,
                 UIControls.Localization.Resources.TicketActivationMessage,
                e1 =>
                {
                    if ((e1.DialogResult ?? false))
                    {
                        _model.ActivateCurrentTicket();
                    }
                });
        }

        private void SaveAnketaButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SaveCustomer();
        }

        private void PrintCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer.SerializedCustomerCards.Count == 1)
            {
                _repMan.ProcessPdfReport(() => _model.ClientContext.GenerateCardContractReport(_model.CurrentCustomer.ActiveCard.CardBarcode));
            }
            else
            {
                _repMan.ProcessPdfReport(() => _model.ClientContext.GenerateCardUpgradeReport(_model.CurrentCustomer.ActiveCard.CardBarcode));
            }
        }

        private void CancelTreatmentEventsButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Confirm(UIControls.Localization.Resources.CancelSelected,
                 String.Format(UIControls.Localization.Resources.CancelTreatmentEvents, CustomerBookingsView.SelectedItems.Count),
                        e1 =>
                        {
                            if ((e1.DialogResult ?? false))
                            {
                                _model.CancelTreatmentEvents(CustomerBookingsView.SelectedItems);
                            }
                        });
        }

        private void NewEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer != null)
            {
                NavigationManager.MakeScheduleRequest(new ScheduleRequestParams { Customer = _model.CurrentCustomer, OnClose = () => _model.RefreshEvents() });
            }
        }

        private void PrintTicketContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTicket == null) return;
            _repMan.ProcessPdfReport(() => _model.ClientContext.GenerateLastTicketSaleReport(_model.CurrentTicket.Id));
        }

        private void PrintTicketInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTicket == null) return;
            _repMan.ProcessPdfReport(() => _model.ClientContext.GenerateTicketVatReport(_model.CurrentTicket.Id));
        }

        private void TreatmentsLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTicket == null) return;
            NavigationManager.MakeTicketRemainReportRequest(_model.CurrentTicket.Number);
        }

        private void TicketEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentTicket == null) return;
            ProcessUserDialog<EditTicketWindow>(() => _model.SelectClient(_model.CurrentCustomer.Id), new ParameterOverride("ticket", _model.CurrentTicket));
        }

        private void PaymentDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var item = (sender as RadGridView).SelectedItem as TicketPayment;
                if (item != null)
                {
                    _repMan.ProcessPdfReport(() => _model.ClientContext.GenerateTicketReceiptReport(item.Id));
                }
            }
        }

        private void EventsViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = RowClicked(e);
            if (row != null)
            {
                var item = row.DataContext as TreatmentEvent;
                if (item != null)
                {
                    TonusWindow.Prompt(UIControls.Localization.Resources.Comment,
                         UIControls.Localization.Resources.ProvideComment,
                         item.Comment ?? "",
                        ev =>
                        {
                            if (ev.DialogResult ?? false)
                            {
                                _model.EditEventComment(item, ev.TextResult.Trim());
                            }
                        });
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_model.CurrentCustomer == null)
            {
                TonusWindow.Alert("Не выбран клиент", "Выберите клиента для того, чтобы загрузить фотографию!");
            }
            else
            {
                ProcessUserDialog<UploadImage>(() => _model.UpdateCustomerImage(), new ParameterOverride("customer", _model.CurrentCustomer));
            }
        }

        private void PaymentCreditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer != null && _model.CurrentTicket != null)
            {
                Tickets.ProcessCreditTicketPayment(() => _model.SelectClient(_model.CurrentCustomer.Id),
                    ClientContext,
                    _model.CurrentCustomer,
                    _model.CurrentTicket);
            }
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer == null) return;
            _repMan.ExportObjectToExcel($"Запись на услуги клиента {_model.CurrentCustomer.FullName}",
            CustomerBookingsView.Items.OfType<TreatmentEvent>().ToList(),
            new ColumnInfo<TreatmentEvent>("Дата", i => i.VisitDate.ToString("dd.MM.yyyy")),
            new ColumnInfo<TreatmentEvent>("Начало", i => i.VisitDate.ToString("HH:mm")),
            new ColumnInfo<TreatmentEvent>("Окончание", i => i.EndTime.ToString("HH:mm")),
            new ColumnInfo<TreatmentEvent>("Абонемент", i => i.SerializedTicketNumber),
            new ColumnInfo<TreatmentEvent>("Тип процедуры", i => i.SerializedTreatmentTypeName),
            new ColumnInfo<TreatmentEvent>("Тренажер", i => i.SerializedTreatmentName)
            //new ColumnInfo<TreatmentEvent>("Статус", i => i.StatusText),
            //new ColumnInfo<TreatmentEvent>("Цена", i => (int)i.Price),
            //new ColumnInfo<TreatmentEvent>("Списано", i => (int)i.Cost),
            //new ColumnInfo<TreatmentEvent>("Комментарий", i => i.Comment)
            );
        }

        private void UnmissEventButton_Click(object sender, RoutedEventArgs e)
        {
            _model.UnmissTreatmentEvents(CustomerBookingsView.SelectedItems);
        }

        private void NoContrasButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SetNoContras();
        }

        private void SaveContrasButton_Click(object sender, RoutedEventArgs e)
        {
            _model.SaveContras();
        }

        private void NewSmartEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer != null)
            {
                NavigationManager.MakeSmartScheduleRequest(new ScheduleRequestParams { Customer = _model.CurrentCustomer, OnClose = () => _model.RefreshEvents() });
            }
        }

        private void ChangeInvitor(object sender, RoutedEventArgs e)
        {
            if (_model.CurrentCustomer == null) return;
            ProcessUserDialog<ChangeInvitorDialog>(() => { _model.SelectClient(_model.CurrentCustomer.Id); }, new ParameterOverride("customer", _model.CurrentCustomer));
        }
    }
}


