using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewtonicServer.Server;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows;
using System.Configuration;
using TonusClub.ServiceModel;
using System.IO;
using System.Runtime.Serialization;

namespace NewtonicServer
{
    class ServerModel : IDisposable
    {
        NewtonicServer.Server.Server _srv = new NewtonicServer.Server.Server();
        public List<ClientInfo> Clients = new List<ClientInfo>();
        public ICollectionView ClientsView { get; set; }
        List<string> _log = new List<string>();
        public ICollectionView LogView { get; set; }

        public string Port { get; set; }

        private Dispatcher Dispatcher;

        TextProcessor textProcessor;
        public HardwareProcessor hwProcessor;

        public ServerModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            ClientsView = CollectionViewSource.GetDefaultView(Clients);
            LogView = CollectionViewSource.GetDefaultView(_log);
            Port = ConfigurationManager.AppSettings.Get("ServerPort") ?? "4505";
            _srv.ClientConnected += new EventHandler<GuidEventArgs>(_srv_ClientConnected);
            _srv.ClientDisonnected += new EventHandler<GuidEventArgs>(_srv_ClientDisonnected);
            _srv.Received += new EventHandler<MessageEventArgs>(_srv_Received);
            textProcessor = new TextProcessor(_srv);
            hwProcessor = new HardwareProcessor(textProcessor);

            hwProcessor.Start += new EventHandler<GuidEventArgs>(hwProcessor_Start);
            hwProcessor.Stop += new EventHandler<GuidEventArgs>(hwProcessor_Stop);
            LoadTreatments();

            Logger.OnLog += new EventHandler<StringEventArgs>(Logger_OnLog);

            int port;
            if (!Int32.TryParse(Port, out port))
            {
                return;
            }
            _srv.Start(port);
            Logger.Log("Сервер запущен " + DateTime.Now.ToString("d.MM.yyyy HH:mm:ss"));
        }

        void Logger_OnLog(object sender, StringEventArgs e)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                _log.Add(String.Format("[{0:H:mm:ss}] {1} : {2}", DateTime.Now, sender.ToString(), e.String));
                LogView.Refresh();
            }));
        }

        void hwProcessor_Stop(object sender, GuidEventArgs e)
        {
            var cli = Clients.FirstOrDefault(i => i.HardwareId == e.Id);
            if (cli != null && cli.IsOnline)
            {
                Logger.Log(cli.Treatment.DisplayName + " остановлен");
                Send(cli.HardwareId, new byte[] { 2 });
            }
        }

        void hwProcessor_Start(object sender, GuidEventArgs e)
        {
            var cli = Clients.FirstOrDefault(i => i.HardwareId == e.Id);
            if (cli != null && cli.IsOnline)
            {
                Logger.Log(cli.Treatment.DisplayName + " запущен");
                Send(cli.HardwareId, new byte[] { 1 });
            }
        }

        void _srv_ClientDisonnected(object sender, GuidEventArgs e)
        {
            var cli = Clients.FirstOrDefault(i => i.HardwareId == e.Id);
            if (cli != null)
            {
                cli.HardwareId = Guid.Empty;
                ClientContext.SetTreatmentOnline(cli.Treatment.Id, false);
            }
        }

        private void LoadTreatments()
        {
            var tres = ClientContext.GetAllTreatments().ToList();
            tres = tres.Where(i => !String.IsNullOrEmpty(i.MacAddress) && i.UseController).ToList();
            tres.ForEach(tre =>
                {
                    var mcs = tre.MacAddress.Split(',').Select(i => i.Trim()).ToArray();
                    if (mcs.Length > 1)
                    {
                        foreach (var mc in mcs)
                        {
                            var tre1 = Clone<Treatment>(tre);
                            tre1.MacAddress = mc;
                            Dispatcher.Invoke(new Action(() => { Clients.Add(new ClientInfo(tre1)); }));

                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => { Clients.Add(new ClientInfo(tre)); }));


                    }
                });
        }

        void _srv_Received(object sender, MessageEventArgs e)
        {
            var cli = Clients.FirstOrDefault(i => i.HardwareId == e.Id);
            if (cli == null)
            {
                var mac = String.Join(":", e.Bytes);
                var tre = Clients.FirstOrDefault(i => i.Treatment.MacAddress.Split(',').Select(j => j.Trim()).Any(j => j == mac));
                if (tre != null)
                {
                    tre.HardwareId = e.Id;
                    Logger.Log("Тренажер идентифицирован (" + tre.Treatment.DisplayName + ")");
                    SetTreatmentOnline(tre.Treatment.Id, true);
                    if (tre.IsOnline && tre.CurrentPlan != null)
                    {
                        hwProcessor_Start(null, new GuidEventArgs { Id = tre.Treatment.Id });
                    }
                    cli = Clients.FirstOrDefault(i => i.HardwareId == e.Id);
                    textProcessor.SetText(cli, "Добро пожаловать в ТОНУС-КЛУБ!", tre.ToString(), 0);
                    InitCurrentTreatment(cli);
                }
                else
                {
                    _srv.IgnoreClient(e.Id);
                    Logger.Log("Тренажер не идентифицирован. " + mac);
                }
            }
            else
            {
                if (e.Bytes.Length == 12)
                {
                    var str = String.Join("", e.Bytes.Skip(4).Select(i => (char)i));
                    var cardnum = System.Int32.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
                    if (cli != null && !cli.StateText.Contains(cardnum.ToString()))
                    {
                        Logger.Log(cli.ToString() + ": приложена карта " + cardnum);
                    }
                    ProcessCard(cli, cardnum);
                }
                else
                {
                    Logger.Log(cli.ToString() + " : Получено сообщение: " + String.Join(", ", e.Bytes));
                }
            }
        }

        private void InitCurrentTreatment(ClientInfo cli)
        {
            if (cli.Treatment.MaxCustomers > 1) return;
            var te = ClientContext.GetCurrentTreatmentEvent(cli.Treatment.Id);
            if (te != null)
            {
                hwProcessor.StartTreatment(cli, te);
            }
        }

        private void SetTreatmentOnline(Guid treatmnetId, bool isOnline)
        {
            ClientContext.SetTreatmentOnline(treatmnetId, isOnline);
        }

        private void ProcessCard(ClientInfo cli, int cardnum)
        {
            var customer = ClientContext.GetCustomerByCard(cardnum);
            if (customer != null)
            {
                if (cli.CurrentPlan != null/* && cli.CurrentPlan.CustomerId == customer.Id*/)
                {
                    return;
                }
                if (cli.BlockCardInfo != null && cli.BlockCardInfo.CustomerId == customer.Id && cli.BlockCardInfo.Timeout > DateTime.Now)
                {
                    textProcessor.SetText(cli, "Запись доступна", "не ранее " + cli.BlockCardInfo.Timeout.ToString("H:mm"), 45);
                    return;
                }

                var plan = ClientContext.GetCustomerPlanningForTreatment(customer.Id, cli.Treatment.Id);
                if (plan != null)
                {
                    ProcessPlannedTreatment(cli, customer, plan);
                }
                else
                {
                    ProcessProposal(cli, customer);
                }
            }
            else
            {
                textProcessor.SetText(cli, "Клиент не в клубе!", cardnum.ToString(), 45);
            }
        }

        private void ProcessProposal(ClientInfo cli, Customer customer)
        {
            if (cli.PendingProposal != null && cli.PendingCustomer.Id == customer.Id)
            {
                var treatment = ClientContext.PostNewTreatmentEvent(cli.PendingProposal.TicketId, cli.Treatment.Id, cli.PendingProposal.VisitDate);
                cli.ClearPendingProposal();
                ClientContext.PostTreatmentStart(treatment.Id, treatment.VisitDate);
                hwProcessor.StartTreatment(cli, treatment);
                return;
            }

            var proposal = ClientContext.GetProposal(cli.Treatment.Id, customer.Id);

            if (!String.IsNullOrWhiteSpace(proposal.ErrorMessage))
            {
                textProcessor.SetText(cli, "Ошибка", proposal.ErrorMessage, 90);
            }
            else
            {
                textProcessor.SetText(cli, proposal.Message, proposal.Line2, 90);
                cli.SetPendingProposalConfirmation(proposal, customer, 90);
            }
        }

        private void ProcessPlannedTreatment(ClientInfo cli, Customer customer, TreatmentEvent plan)
        {
            if (cli.Treatment.MaxCustomers > 1 && plan.VisitStatus == 2)
            {
                hwProcessor.StartTreatment(cli, plan);
                return;
            }
            if (cli.PendingEvent != null && cli.PendingEvent.Id == plan.Id)
            {
                ClientContext.PostTreatmentStart(plan.Id, cli.PendingEvent.VisitDate);
                hwProcessor.StartTreatment(cli, cli.PendingEvent);
                return;
            }
            var now = DateTime.Now;
            if (plan.VisitDate < now)
            {
                //Необходимо проверять, можно ли подержать услугу подольше
                var len = ClientContext.CorrectAvailableTreatmentLength(plan.Id);// plan.SerializedDuration - (int)((DateTime.Now - plan.VisitDate).TotalMinutes);
                var str = String.Format("Вы записаны на {0:H:mm}, сейчас {1:H:mm}, занятие будет длиться {2} минут и закончится в {3:H:mm}. Для подтверждения повторно приложите карту.",
                    plan.VisitDate, DateTime.Now, len, DateTime.Now.AddMinutes(len));
                textProcessor.SetText(cli, str, String.Format("{0:H:mm} - {1:H:mm}", DateTime.Now, DateTime.Now.AddMinutes(len)), 120);
            }

            if (plan.VisitDate >= now)
            {
                var str = String.Format("Вы записаны на {0:H:mm}, сейчас {1:H:mm}, занятие закончится в {2:H:mm}. Для подтверждения повторно приложите карту.",
                    plan.VisitDate, now, now.AddMinutes(plan.SerializedDuration));
                plan.VisitDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                textProcessor.SetText(cli, str, String.Format("{0:H:mm} - {1:H:mm}", DateTime.Now, plan.VisitDate.AddMinutes(plan.SerializedDuration)), 120);
            }
            cli.SetPendingConfirmation(customer, plan, 60);
        }

        //private void Log(string p)
        //{
        //    _log.Add(p);
        //    Dispatcher.Invoke(new Action(() => LogView.Refresh()));
        //}

        void _srv_ClientConnected(object sender, GuidEventArgs e)
        {
            //Clients.Add(e.Id, new ClientInfo { HardwareId = e.Id });
            //Dispatcher.Invoke(new Action(()=>ClientsView.Refresh()));
            textProcessor.SetText(new ClientInfo(new Treatment { Tag = "Неизвестен" }) { HardwareId = e.Id }, "Идентификация...", "", 0);
            //Log("Новый клиент подключен, id = " + e.Id.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }

        }

        public void Send(Guid id, byte[] i)
        {
            _srv.Send(id, i);
        }


        public void Dispose()
        {
            Clients.ForEach(i =>
            {
                if (i.Treatment != null && i.HardwareId != Guid.Empty)
                {
                    textProcessor.SetText(i, "Тренажер", "отключен", 100);
                    _srv.Send(i.HardwareId, new byte[] { 2 });
                    ClientContext.SetTreatmentOnline(i.Treatment.Id, false);
                }
            });
            _srv.Stop();
        }

        public static T Clone<T>(object obj)
    where T : class
        {
            if (obj == null) return null;
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            T newObject = (T)dcSer.ReadObject(memoryStream);
            return newObject;
        }

    }
}
