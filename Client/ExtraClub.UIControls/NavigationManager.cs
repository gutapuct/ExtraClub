using System;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.ParameterClasses;
using ExtraClub.Infrastructure;

namespace ExtraClub.UIControls
{
    public static class NavigationManager
    {
        public static event EventHandler<ClientEventArgs> ClientRequest;
        public static void MakeClientRequest(Guid clientId)
        {
            if(ClientRequest != null)
            {
                ClientRequest.Invoke(null, new ClientEventArgs(clientId));
            }
        }

        public static event EventHandler<ScheduleRequestEventArgs> ScheduleRequest;
        public static void MakeScheduleRequest(ScheduleRequestParams scheduleRequestParams)
        {
            if(ScheduleRequest != null)
            {
                ScheduleRequest.Invoke(null, new ScheduleRequestEventArgs(scheduleRequestParams));
            }
        }

        public static event EventHandler<ScheduleRequestEventArgs> SmartScheduleRequest;
        public static void MakeSmartScheduleRequest(ScheduleRequestParams scheduleRequestParams)
        {
            if(SmartScheduleRequest != null)
            {
                SmartScheduleRequest.Invoke(null, new ScheduleRequestEventArgs(scheduleRequestParams));
            }
        }

        public static event EventHandler<ClientEventArgs> ClientInRequest;
        public static void MakeClientInRequest(Guid customerId, Action okAction)
        {
            if(ClientInRequest != null)
            {
                ClientInRequest.Invoke(null, new ClientEventArgs(customerId) { OkAction = okAction });
            }
        }

        public static event EventHandler<ClientEventArgs> ClientOutRequest;
        public static void MakeClientOutRequest(Guid customerId, Action okAction)
        {
            if(ClientOutRequest != null)
            {
                ClientOutRequest.Invoke(null, new ClientEventArgs(customerId) { OkAction = okAction });
            }
        }

        public static event EventHandler<ClientEventArgs> NewChildRequest;
        public static void MakeNewChildRequest(Guid customerId)
        {
            if(NewChildRequest != null)
            {
                NewChildRequest.Invoke(null, new ClientEventArgs(customerId));
            }
        }

        public static event EventHandler<NewSolariumVisitEventArgs> NewSolariumVisitRequest;
        public static void MakeNewSolariumVisitRequest(NewSolariumVisitParams solRequestParams)
        {
            if(NewSolariumVisitRequest != null)
            {
                NewSolariumVisitRequest.Invoke(null, new NewSolariumVisitEventArgs(solRequestParams));
            }
        }

        public static event EventHandler ActivateSolariumScheduleRequest;
        public static void MakeActivateSolariumScheduleRequest()
        {
            if(ActivateSolariumScheduleRequest != null)
            {
                ActivateSolariumScheduleRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler<ClientEventArgs> BarRequest;
        public static void MakeBarRequest(Guid customerId)
        {
            if(BarRequest != null)
            {
                BarRequest.Invoke(null, new ClientEventArgs(customerId));
            }
        }

        public static event EventHandler ActivateTreatmentsScheduleRequest;
        public static void MakeActivateTreatmentsScheduleRequest()
        {
            if(ActivateTreatmentsScheduleRequest != null)
            {
                ActivateTreatmentsScheduleRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler<ClientEventArgs> NewClientRequest;
        public static void MakeNewClientRequest(Action<Guid> onCompeted)
        {
            if(NewClientRequest != null)
            {
                var args = new ClientEventArgs(Guid.Empty) { OnSuccess = onCompeted };
                NewClientRequest.Invoke(null, args);
            }
        }

        public static event EventHandler<ObjectEventArgs> QueryOneEmployee;
        public static void MakeQueryOneEmployee(Action<Employee> onClose)
        {
            if(QueryOneEmployee != null)
            {
                QueryOneEmployee.Invoke(null, new ObjectEventArgs { OnEmployee = onClose });
            }
        }

        public static event EventHandler<ClientEventArgs> NewCustomerNotificationRequest;
        public static void MakeNewCustomerNotificationRequest(Customer customer, Action closeAction)
        {
            if(NewCustomerNotificationRequest != null)
            {
                NewCustomerNotificationRequest.Invoke(null, new ClientEventArgs(customer == null ? Guid.Empty : customer.Id) { OnSuccess = w => closeAction() });
            }
        }

        public static event EventHandler<GuidEventArgs> CustomerNotificationRequest;
        public static void MakeCustomerNotificationTask(Guid notificationId, Action closeAction)
        {
            if(CustomerNotificationRequest != null)
            {
                CustomerNotificationRequest.Invoke(null, new GuidEventArgs { Guid = notificationId, OnClose = closeAction });
            }
        }

        public static event EventHandler<ObjectEventArgs> TaskRequest;
        public static void MakeTaskRequest(OrganizerItem item, Action okAction)
        {
            if(TaskRequest != null)
            {
                TaskRequest.Invoke(null, new ObjectEventArgs { Object = item, OnOkay = okAction });
            }
        }

        public static event EventHandler<ClientEventArgs> NewOutgoingCallRequest;
        public static void MakeNewOutgoingCallRequest(Customer customer, Action closeAction)
        {
            if(NewOutgoingCallRequest != null)
            {
                NewOutgoingCallRequest.Invoke(null, new ClientEventArgs(customer == null ? Guid.Empty : customer.Id) { OnSuccess = w => closeAction() });
            }
        }

        public static event EventHandler<ObjectEventArgs> ActivateLoginsRequest;
        public static void MakeActivateLoginsRequest(Guid empId)
        {
            if(ActivateLoginsRequest != null)
            {
                ActivateLoginsRequest.Invoke(null, new ObjectEventArgs { Object = empId });
            }
        }

        public static event EventHandler<GuidEventArgs> CustomerTargetRequest;
        public static void MakeCustomerTargetRequest(Guid targetId)
        {
            if(CustomerTargetRequest != null)
            {
                CustomerTargetRequest.Invoke(null, new GuidEventArgs { Guid = targetId });
            }
        }

        public static event EventHandler<GuidEventArgs> TreatmentEventRequest;
        public static void MakeTreatmentEventRequest(Guid targetId)
        {
            if(TreatmentEventRequest != null)
            {
                TreatmentEventRequest.Invoke(null, new GuidEventArgs { Guid = targetId });
            }
        }

        public static event EventHandler<GuidEventArgs> TicketRequest;
        public static void MakeTicketRequest(Guid ticketId)
        {
            if(TicketRequest != null)
            {
                TicketRequest.Invoke(null, new GuidEventArgs { Guid = ticketId });
            }
        }

        public static event EventHandler<GuidEventArgs> CustomerCardRequest;
        public static void MakeCustomerCardRequest(Guid cardId)
        {
            if(CustomerCardRequest != null)
            {
                CustomerCardRequest.Invoke(null, new GuidEventArgs { Guid = cardId });
            }
        }

        public static event EventHandler<GuidEventArgs> GoodSalesRequest;
        public static void MakeGoodSalesRequest(Guid saleId)
        {
            if(GoodSalesRequest != null)
            {
                GoodSalesRequest.Invoke(null, new GuidEventArgs { Guid = saleId });
            }
        }

        public static event EventHandler<GuidEventArgs> SpendingRequest;
        public static void MakeSpendingRequest(Guid spendingId)
        {
            if(SpendingRequest != null)
            {
                SpendingRequest.Invoke(null, new GuidEventArgs { Guid = spendingId });
            }
        }

        public static event EventHandler<GuidEventArgs> EmployeeRequest;
        public static void MakeEmployeeRequest(Guid employeeId)
        {
            if(EmployeeRequest != null)
            {
                EmployeeRequest.Invoke(null, new GuidEventArgs { Guid = employeeId });
            }
        }

        public static event EventHandler<ObjectEventArgs> TicketRemainReportRequest;
        public static void MakeTicketRemainReportRequest(string ticketNumber)
        {
            if(TicketRemainReportRequest != null)
            {
                TicketRemainReportRequest.Invoke(null, new ObjectEventArgs { Object = ticketNumber });
            }
        }

        public static event EventHandler<GuidEventArgs> NewTicketRequest;
        public static void MakeNewTicketRequest(Guid customerId, Action onClose)
        {
            if(NewTicketRequest != null)
            {
                NewTicketRequest.Invoke(null, new GuidEventArgs { Guid = customerId, OnClose = onClose });
            }
        }

        public static event EventHandler<ObjectEventArgs> UserActionsReportRequest;
        public static void MakeUserActionsReportRequest(string userName)
        {
            if(UserActionsReportRequest != null)
            {
                UserActionsReportRequest.Invoke(null, new ObjectEventArgs { Object = userName });
            }
        }

        public static event EventHandler<GuidEventArgs> ParallelSigningRequest;
        public static void MakeParallelSigningRequest(Guid treatmentEventId, Action onComplete)
        {
            if(ParallelSigningRequest != null)
            {
                ParallelSigningRequest.Invoke(null, new GuidEventArgs { Guid = treatmentEventId, OnClose = onComplete });
            }
        }


        public static event EventHandler<GuidEventArgs> NavigateToJob;
        public static void OnNavigateToJob(Guid jobId)
        {
            if(NavigateToJob != null)
            {
                NavigateToJob.Invoke(null, new GuidEventArgs { Guid = jobId });
            }
        }

        public static event EventHandler<StringEventArgs> NavigateToCashFlow;
        public static void OnNavigateToCashFlow(string empName)
        {
            if(NavigateToCashFlow != null)
            {
                NavigateToCashFlow.Invoke(null, new StringEventArgs { Value = empName });
            }
        }

        public static event EventHandler CloseSplashRequest;
        internal static void CloseSplash()
        {
            if(CloseSplashRequest != null)
            {
                CloseSplashRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler RefreshBarRequest;
        public static void RefreshBar()
        {
            if(RefreshBarRequest != null)
            {
                RefreshBarRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler RefreshGoodsRequest;
        public static void RefreshGoods()
        {
            if(RefreshGoodsRequest != null)
            {
                RefreshGoodsRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler<ObjectEventArgs> CallTaskRequest;
        public static void MakeCallTaskRequest(Guid[] customerIds)
        {
            if(CallTaskRequest != null)
            {
                CallTaskRequest.Invoke(null, new ObjectEventArgs { Object = customerIds });
            }
        }

        public static event EventHandler<EventArgs> AllCustomersReportRequest;
        public static void MakeAllCustomersReportRequest()
        {
            if(AllCustomersReportRequest != null)
            {
                AllCustomersReportRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler<EventArgs> AllTicketsReportRequest;
        public static void MakeAllTicketsReportRequest()
        {
            if(AllTicketsReportRequest != null)
            {
                AllTicketsReportRequest.Invoke(null, new EventArgs());
            }
        }

        public static event EventHandler<GuidEventArgs> DownloadFileRequest;
        public static void MakeDownloadFileRequest(Guid g)
        {
            if(DownloadFileRequest != null)
            {
                DownloadFileRequest.Invoke(null, new GuidEventArgs { Guid = g });
            }
        }

        public static event EventHandler<ScheduleRequestEventArgs> ConsultationRequest;
        public static void MakeConsultationRequest(ScheduleRequestParams scheduleRequestParams)
        {
            if(ConsultationRequest != null)
            {
                ConsultationRequest.Invoke(null, new ScheduleRequestEventArgs(scheduleRequestParams));
            }
        }
    }

    public class StringEventArgs : EventArgs
    {
        public string Value { get; set; }
    }
    public class GuidEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Action OnClose { get; set; }
    }
    public class ClientEventArgs : EventArgs
    {
        public Guid ClientId { get; set; }
        public Action<Guid> OnSuccess { get; set; }
        public Action OkAction { get; set; }

        public ClientEventArgs(Guid clientId)
        {
            ClientId = clientId;
        }
    }
    public class ScheduleRequestEventArgs : EventArgs
    {
        public ScheduleRequestParams ScheduleRequestParams { get; private set; }

        public ScheduleRequestEventArgs(ScheduleRequestParams scheduleRequestParams)
        {
            ScheduleRequestParams = scheduleRequestParams;
        }
    }
    public class NewSolariumVisitEventArgs : EventArgs
    {
        public NewSolariumVisitParams NewSolariumVisitParams { get; private set; }

        public NewSolariumVisitEventArgs(NewSolariumVisitParams newSolariumVisitParams)
        {
            NewSolariumVisitParams = newSolariumVisitParams;
        }
    }
    public class ObjectEventArgs : EventArgs
    {
        public object Object { get; set; }
        public Action<Employee> OnEmployee { get; set; }

        public Action OnOkay { get; set; }
    }
}
