using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using ExtraClub.CashRegisterModule;
using ExtraClub.UIControls;
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.Extensions;
using Telerik.Windows.Controls;
using System.ServiceModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.Clients.Views.Windows;
using ExtraClub.Infrastructure;
using System.IO;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.Clients.Views.ContainedControls
{
    public partial class RegisterComeOut
    {
        #region DataContext
        Visibility _programGridVisibility = Visibility.Collapsed;

        public class RecommendedDrinkables
        {
            public RecommendedDrinkables()
            {
                AddRecommendation("Пейте детокс-напиток «Дольче» и почувствуйте легкость. Активные компоненты способствуют нормальной работе ЖКТ, очищают от шлаков и токсинов, стимулируют процесс похудения.");
                AddRecommendation("Детокс-напиток «Дольче» - первый шаг на пути к красоте и здоровью. Он мягко подготавливает организм к дальнейшему похудению.");
                AddRecommendation("Замените перекус блокатором калорий «Дикая вишня» и Вы снизите аппетит, калорийность рациона и повысите эффективность жиросжигающих тренировок.");
                AddRecommendation("Блокатор калорий «Дикая вишня» позволяет удерживать вес и закрепить достигнутый результат.");
                AddRecommendation("Замените Ваш обычный кофе латте «Жизненная сила». Низкая калорийность, активные жиросжигающие компоненты, минералы и витамины в его составе сделают Ваш перекус не только вкусным и полезным, но и помогут скорее похудеть.");
                AddRecommendation("Кофе латте «Жизненная сила» - отличное дополнение к серии напитков. Активные компоненты способствуют обмену веществ и снижению тяги к сладкому.");
                AddRecommendation("Пейте «Шоколадное дерево» перед тренировкой и Вы почувствуете прилив сил, энергии, а уже через несколько недель будут видны первые результаты тренировок.");
                AddRecommendation("Жиросжигающий напиток «Шоколадное дерево» активизирует процесс расщепления жиров и снижению веса без вреда для организма.");
                AddRecommendation("«Дикая вишня» + «Шоколадное дерево» = быстрое похудение и закрепление результата без диет и голодовок!");
                AddRecommendation("«Шоколадное дерево» + «Дикая вишня» = быстрое похудение, снижение аппетита, вкусные перекусы, прилив энергии. Откройте для себя вкусное и полезное похудение!");
                AddRecommendation("«Шоколадное дерево» + «Дольче» = эффективное похудение и очищение организма от шлаков и токсинов. Быстрое похудение и правильное питание не означает отказ от вкусного!");
                AddRecommendation("«Дольче» + «Шоколадное дерево» = идеальное сочетание для тех, кто хочет похудеть быстро и здорово!");
                AddRecommendation("«Дольче» + латте «Жизненная сила» = мягкое очищение организма от лишнего и энергия для достижения любой цели!");
                AddRecommendation("Латте «Жизненная сила» + «Дольче» = снизят тягу к сладкому и помогут ощутить легкость.");
                AddRecommendation("«Дольче» + «Дикая вишня» = лучший комплекс для очищения организма, достижения легкости и закрепления результата!");
                AddRecommendation("«Дикая вишня» + «Дольче» = комфортный переход на правильное питание для тех, кто заботится о своем здоровье и комфорте.");
                AddRecommendation("«Шоколадное дерево» + латте «Жизненная сила» = Вы полны бодрости и готовы к достижению любой цели!");
                AddRecommendation("Латте «Жизненная сила» + «Шоколадное дерево» = вкусное быстрое похудение и помощь организму при переходе на правильное питание.");
                AddRecommendation("«Дикая вишня» + латте «Жизненная сила» = помогают закрепить полученный результат и не «сорваться на сладенькое».");
                AddRecommendation("Латте «Жизненная сила» + «Дикая вишня» = идеальные перекусы во время похудения и закрепления достигнутого результата.");

                AddRecommendation("Используйте антицеллюлитный крем «Магия» после процедуры термопохудения и Вы заметите, как быстро уменьшаются видимые признаки целлюлита, а кожа становится упругой и эластичной. ");
                AddRecommendation("Активные компоненты антицеллюлитного крема «Магия» усиливают эффект от тренировок, направленных на жиросжигание и избавление от целлюлита. Используйте крем после тренировки, и результат не заставит себя долго ждать!");
                AddRecommendation("Лимфодренажный гель «Провоканте» -прекрасное дополнение к процедурам массажа. Уникальные компоненты помогают снять отечность, избавиться от усталости и тяжести.");
                AddRecommendation("Коррекция веса, устранение отечности, целлюлита, профилактика варикоза – любую цель Вы достигните быстрее, используя лимфодренажный гель «Провоканте».");
                AddRecommendation("Омолаживающий крем «Прима» -незаменимое средство для каждой женщины, заботящейся о состоянии кожи тела. Компоненты крема увлажняют, питают, тонизируют, омолаживают кожу. ");
                AddRecommendation("Омолаживающий крем «Прима», благодаря уникальным компонентам, уже с первого применения улучшает состояние кожи: подтягивает, тонизирует, придает мягкость, упругость и сияние.");
                AddRecommendation("«Прима» + «Магия» = в сочетании с тренировками дают потрясающий результат: за короткий срок уменьшаются объемы, исчезает целлюлит, а кожа становится упругой и подтянутой.");
                AddRecommendation("«Магия» + «Прима» = ежедневный уход и регулярные тренировки помогут Вам в кратчайший срок достичь любой цели. Уникальные компоненты кремов в несколько раз усиливают эффект, полученный от smart-тренировок.");
                AddRecommendation("«Прима» + «Провоканте» = идеальный вариант для тех, кто весь день на ногах.Комплекс поможет снять усталость и напряжение, тонизирующие и питательные компоненты сделают кожу упругой и сияющей.");
                AddRecommendation("«Провоканте» + «Прима» = настоящий подарок Вашей коже.В сочетании со smart-тренировками снимают напряжение, тонизируют кожу, делают ее упругой и сияющей.");
                AddRecommendation("«Шоколадное дерево» + «Магия» = дополнительные средства ухода способствуют быстрому и здоровому похудению.Напитки и крема очищают и оздоравливают организм как снаружи, так и изнутри.");
                AddRecommendation("«Магия» + «Шоколадное дерево» = стимулируют жиросжигание, обменные процессы, мягко и эффективно воздействуя изнутри и снаружи. Это идеальный комплекс для тех, кто стремится похудеть быстро и без вреда для организма.");
            }

            private List<string> Recommendations = new List<string>();

            private void AddRecommendation (string recommendation)
            {
                if (!String.IsNullOrWhiteSpace(recommendation))
                {
                    Recommendations.Add(recommendation);
                }
            }

            public string GetRandomRecommendation()
            {
                var countRecommendations = Recommendations.Count;
                if (countRecommendations == 0) return String.Empty;

                var random = new Random();
                return Recommendations[random.Next(0, countRecommendations-1)];
            }
        }

        public Visibility ProgramGridVisibility
        {
            get
            {
                return _programGridVisibility;
            }
            set
            {
                _programGridVisibility = value;
                OnPropertyChanged("IsComeInEnabled");
            }
        }

        public BitmapSource CustomerImage { get; set; }


        private void RefreshFirstVisit()
        {
            if (_customer != null)
            {
                IsFirstVisit = _context.IsFirstVisitEnabled(_customer.Id);
                OnPropertyChanged("IsFirstVisit");
            }
        }

        public bool IsFirstVisit { get; set; }

        Customer _customer;
        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                RefreshFirstVisit();
                if (_customer != null && _context.GetCustomerChildren(_customer.Id).Any(i => !i.OutById.HasValue && i.DivisionId == _context.CurrentDivision.Id))
                {
                    ChildRoomDiv.Visibility = Visibility.Visible;
                }
                if (_customer != null)
                {
                    if (_customer.ShelfNumber.HasValue)
                    {
                        if (AuthorizationManager.SetElementVisible(ShelfReturn))
                        {
                            ShelfReturn.Content = String.Format(UIControls.Localization.Resources.ShelfKeyReturned, Customer.ShelfNumber);
                        }
                    }
                    if (_customer.SafeNumber.HasValue)
                    {
                        if (AuthorizationManager.SetElementVisible(SafeReturn))
                        {
                            SafeReturn.Content = String.Format(UIControls.Localization.Resources.SafeKeyReturned, Customer.SafeNumber);
                        }
                    }
                    var bs = _context.GetCustomerImage(Customer.Id);
                    if (bs != null)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = new MemoryStream(bs);
                        bitmapImage.EndInit();
                        CustomerImage = bitmapImage;
                    }
                    else
                    {
                        CustomerImage = null;
                    }
                }
                else
                {
                    CustomerImage = null;
                }

                OnPropertyChanged("Customer");
            }
        }

        public ICollectionView TreatmentEvents { get; private set; }
        private List<TreatmentEvent> treatmentEvents = new List<TreatmentEvent>();

        string _cardNumber;
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value)) return;
                if (Customer == null)
                {
                    Customer = _context.GetCustomerByCard(Int32.Parse(value), true);
                    if (Customer != null && String.IsNullOrEmpty(Customer.PresenceStatusText))
                    {
                        Customer = null;
                        ExtraWindow.Alert(new DialogParameters
                        {
                            Header = UIControls.Localization.Resources.Error,
                            Content = UIControls.Localization.Resources.CustomerNotInClub,
                            OkButtonContent = UIControls.Localization.Resources.Ok,
                            Owner = this
                        });
                        return;
                    }

                    RefreshEvents();
                }
                _cardNumber = value;
            }
        }

        bool _post = true;
        public bool IsComeInEnabled
        {
            get
            {
                return _post;
            }
            set
            {
                _post = value;
                OnPropertyChanged("IsComeInEnabled");
            }
        }

        string _prog;
        public string ProgramName
        {
            get
            {
                return _prog;
            }
            set
            {
                ProgramGridVisibility = String.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible;
                _prog = value;
                OnPropertyChanged("ProgramName");
            }
        }

        #endregion

        CashRegisterManager _cashMan;
        IReportManager _repMan;
        IUnityContainer _cont;

        public RegisterComeOut( CashRegisterManager cashMan, Customer customer, IReportManager repMan, IUnityContainer cont)
        {
            _cont = cont;
            if (customer.Id == Guid.Empty)
            {
                customer = null;
            }
            else
            {
                customer = _context.GetCustomer(customer.Id, true);
            }
            InitializeComponent();
            Customer = customer;
            _cashMan = cashMan;
            _repMan = repMan;
            TreatmentEvents = CollectionViewSource.GetDefaultView(treatmentEvents);
            RefreshEvents();
            this.DataContext = this;

            var post = AppSettingsManager.GetSetting("PostReceiptSetting");
            if (String.IsNullOrEmpty(post) || post == "0")
            {
                PrintFR.IsChecked = true;
            }
            else if (post == "1")
            {
                PrintPDF.IsChecked = true;
            }
            else
            {
                DoNotPrint.IsChecked = true;
            }

            Owner = Application.Current.MainWindow;
        }

        private void RefreshEvents()
        {
            if (Customer == null) return;
            ProgramName = "";
            treatmentEvents.Clear();
            var evs = _context.GetCustomerEvents(Customer.Id, DateTime.Today, DateTime.Today.AddDays(1).AddSeconds(-1), false).Where(i => i.VisitStatus == 2 || i.VisitStatus == 3);
            treatmentEvents.AddRange(evs);
            TreatmentEvents.Refresh();
            foreach (var e in treatmentEvents)
            {
                if (e.ProgramId.HasValue)
                {
                    ProgramName = e.SerializedProgramName;
                }
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Customer == null)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = UIControls.Localization.Resources.NeedCustomerError,
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return;
            }
            if (Customer.RurDepositValue < 0)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = String.Format(UIControls.Localization.Resources.DepositLowError, -Customer.RurDepositValue),
                    OkButtonContent = UIControls.Localization.Resources.Ok,
                    Owner = this
                });
                return;
            }
            if (Customer.ActiveCard != null && (String.IsNullOrWhiteSpace(CardNumber) || CardNumber != Customer.ActiveCard.CardBarcode))
            {
                ExtraWindow.Alert(new DialogParameters
                    {
                        Header = UIControls.Localization.Resources.Error,
                        Content = String.Format(UIControls.Localization.Resources.CardNeeded, Customer.ActiveCard.CardBarcode),
                        OkButtonContent = UIControls.Localization.Resources.Ok,
                        Owner = this
                    });
                return;
            }
            if (Customer.ShelfNumber.HasValue && !(ShelfReturn.IsChecked ?? false))
            {
                ExtraWindow.Confirm(
                    UIControls.Localization.Resources.Shelf,
                    UIControls.Localization.Resources.NoKeyFine,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ContinueCommit0();
                        }
                    });
            }
            else
            {
                ContinueCommit0();
            }
        }

        private void ContinueCommit0()
        {
            if (Customer.SafeNumber.HasValue && !(SafeReturn.IsChecked ?? false))
            {
                ExtraWindow.Confirm(UIControls.Localization.Resources.Safe,
                     UIControls.Localization.Resources.NoKeyFine,
                    e1 =>
                    {
                        if (e1.DialogResult ?? false)
                        {
                            ContinueCommit();
                        }
                    });
            }
            else
            {
                ContinueCommit();
            }
        }

        private void ContinueCommit()
        {
            var visitId = Guid.Empty;

            try
            {
                visitId = _context.RegisterCustomerVisitEnd(Customer.Id, ShelfReturn.IsChecked ?? false, SafeReturn.IsChecked ?? false);
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert(new DialogParameters
                {
                    Header = UIControls.Localization.Resources.Error,
                    Content = ex.Reason,
                    Owner = this
                });
                return;
            }

            var len = PrintFR.IsChecked ?? false ? 35 : 256;
            var tickets = _context.GetCustomerTickets(Customer.Id).Where(i => i.Status == TicketStatus.Active || i.Status == TicketStatus.Available || i.Status == TicketStatus.Freezed).ToArray();
#if !BEAUTINIKA
            var hasSmart = tickets.Any(i => i.SerializedTicketType.IsSmart);
#else
            var hasSmart = false;
#endif
            var ls = new List<string> { UIControls.Localization.Resources.Visitor };
            ls.AddRange(Customer.FullName.SplitByLen(len));
            if (Customer.ActiveCard != null)
            {
                ls.AddRange(String.Format("{1}: {0}", Customer.ActiveCard.CardBarcode, UIControls.Localization.Resources.CardNumber).SplitByLen(len));
            }
            ls.Add("-----------------------------------");

            if (!hasSmart)
            {
                #if !BEAUTINIKA
                ls.Add(UIControls.Localization.Resources.UnitOuts + ":");
#else
                ls.Add("Акт оказания услуг");

#endif
                var n = 1;
                var sum = 0;
                var uo = _context.GetCustomerUnitCharges(Customer.Id, DateTime.Today,
                    DateTime.Today.AddDays(1).AddSeconds(-1), false);
                foreach (var charge in uo)
                {
                    ls.AddRange(String.Format("{0}. {1}", n++, charge.Reason).SplitByLen(len));
#if BEAUTINIKA
                //ls.AddRange(String.Format("   {1}: {0}", charge.Charge + charge.ExtraCharge, UIControls.Localization.Resources.Sum).SplitByLen(len));
                ls.AddRange(String.Format("   {1}: {0}", charge.SerializedTicketNumber, UIControls.Localization.Resources.Ticket).SplitByLen(len));
                ls.AddRange(String.Format("Эстетист: ______ {0}", charge.SerializedEmployee).SplitByLen(len));
                sum += charge.Charge + charge.ExtraCharge;
#else
                    ls.AddRange(
                        String.Format("   {1}: {0}", charge.Charge, UIControls.Localization.Resources.Sum)
                            .SplitByLen(len));
                    ls.AddRange(
                        String.Format("   {1}: {0}", charge.SerializedTicketNumber,
                            UIControls.Localization.Resources.Ticket).SplitByLen(len));
                    sum += charge.Charge;
#endif
                }
                ls.Add("");
#if BEAUTINIKA
                ls.AddRange(
                    String.Format("Количество проведенных процедур: {0:n0}", uo.Count).SplitByLen(len));
#else
                ls.AddRange(
                    String.Format("{1}: {0:n0}", sum, UIControls.Localization.Resources.TotalTreCost).SplitByLen(len));
#endif
            }

            var bar = _context.GetBarOrdersForCustomer(Customer.Id, DateTime.Today, DateTime.Today);
            if (bar.Count > 0)
            {
                ls.Add(UIControls.Localization.Resources.BarPurchases);

                var n = 1;
                foreach (var gs in bar)
                {
                    if (gs.PriceMoney.HasValue)
                    {
                        ls.AddRange(String.Format("{0}. {1}, {3:n0} {4}, {2:c}", n++, gs.SerializedGoodName, (gs.PriceMoney.Value) * (decimal)gs.Amount, gs.Amount, UIControls.Localization.Resources.Unit).SplitByLen(len));
                    }
                    else
                    {
                        ls.AddRange(String.Format("{0}. {1}, {3:n0} {4}, {2:n0} бон.", n++, gs.SerializedGoodName, (gs.PriceBonus) * (decimal)gs.Amount, gs.Amount, UIControls.Localization.Resources.Unit).SplitByLen(len));
                    }
                }
            }
            ls.AddRange(String.Format("{1}: {0:n0}", Customer.BonusDepositValue, UIControls.Localization.Resources.BonusesAmount).SplitByLen(len));
            ls.AddRange(String.Format("{1}: {0:c}", Customer.RurDepositValue, UIControls.Localization.Resources.YourDeposit).SplitByLen(len));
            ls.Add(UIControls.Localization.Resources.ActiveTickets);
            var n1 = 1;
            foreach (var t in tickets)
            {
#if BEAUTINIKA
                ls.AddRange(String.Format("{0}. {1}, окончание {2:d}, остаток единиц: {3:n0}, остаток доп. единиц: {4:n0}, минут солярия: {5:n0}",
                    n1++, t.Number,
                    t.FinishDate, t.UnitsLeft, t.ExtraUnitsLeft,
                    t.SolariumMinutesLeft).SplitByLen(len));
#else
                ls.AddRange(
                    String.Format(
                        t.SerializedTicketType.IsSmart
                            ? UIControls.Localization.Resources.TicketInfoBulkSmart
                            : UIControls.Localization.Resources.TicketInfoBulk,
                        n1++, t.Number,
                        t.FinishDate, t.UnitsLeft, t.GuestUnitsLeft,
                        t.SolariumMinutesLeft,
                        Math.Floor(t.UnitsLeft/8)).SplitByLen(len));
#endif
            }
            ls.Add("");

            var events = _context.GetCustomerEvents(Customer.Id, DateTime.Today.AddDays(1), DateTime.MaxValue, false);
            if (events.Any())
            {
                var date = events.Select(i => i.VisitDate.Date).Min();
                events = events.Where(i => i.VisitDate.Date == date).OrderBy(i => i.VisitDate).ToList();
                ls.AddRange(String.Format("Следующее посещение запланировано на {0:d MMMM yyyy}", date).SplitByLen(len));
                foreach (TreatmentEvent te in events)
                {
                    ls.AddRange(String.Format("{0:H:mm} {1}", te.VisitDate, te.SerializedTreatmentTypeName).SplitByLen(len));
                }
                ls.Add("");
            }

            ls.AddRange((UIControls.Localization.Resources.AgreedToInfo + ":________").SplitByLen(len));
            ls.Add("");

            var recommendation = new RecommendedDrinkables().GetRandomRecommendation();
            if (!String.IsNullOrWhiteSpace(recommendation))
            {
                ls.AddRange((recommendation).SplitByLen(len));
                ls.Add("");
            }

            if (ChildRoomDiv.Visibility == Visibility.Visible)
            {
                ls.AddRange((UIControls.Localization.Resources.ChildRedeemed + ":_________").SplitByLen(len));
            }
            ls.Add("-----------------------------------");
            Logger.Log("Сгенерирован отчет о посещении для {0}", visitId);
            _context.UpdateVisitReceipt(visitId, Concatenate(ls, '\n'));
            Logger.Log("Сохранен отчет о посещении для {0}", visitId);

            if (PrintFR.IsChecked ?? false)
            {
                if (!_cashMan.PrintText(ls))
                {
                    return;
                }
            }
            if (PrintPDF.IsChecked ?? false)
            {
                _repMan.PrintTextToPdf(ls);
            }


            string post;
            if (PrintFR.IsChecked ?? false)
            {
                post = "0";
            }
            else if (PrintPDF.IsChecked ?? false)
            {
                post = "1";
            }
            else
            {
                post = "2";
            }
            AppSettingsManager.SetSetting("PostReceiptSetting", post);

            DialogResult = true;
            Close();
        }

        private string Concatenate(IEnumerable<string> ls, char separator)
        {
            var sb = new StringBuilder();
            foreach (var i in ls)
            {
                if (sb.Length > 0) sb.Append(separator);
                sb.Append(i);
            }
            return sb.ToString();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CorrectVisited_Click(object sender, RoutedEventArgs e)
        {
            ModuleViewBase.ProcessUserDialog<VisitCorrectionWindow>(_cont, () =>
            {
                RefreshEvents();
                Customer = _context.GetCustomer(Customer.Id, true);
            }, new ParameterOverride("customer", _customer));
        }
    }
}
