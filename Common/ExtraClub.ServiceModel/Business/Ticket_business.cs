using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    partial class Ticket : IInitable
    {

        public Ticket()
        {
            OnDeserialized();
        }

        partial void OnDeserialized()
        {
            PropertyChanged += Ticket_PropertyChanged;
        }
        bool _isInited;
        public void InitDetails()
        {
            if (_isInited) return;
            _isInited = true;
            SerializedTicketType = TicketType;
            SerializedDivision = Division;
            SerializedTicketPayments = TicketPayments;
            SerializedTicketFreezes = TicketFreezes;
            SerializedActivateInstalment = Division.Company.ActivateInstalment;

            InitFinishDate();

            InitUnitsOut();

            ActionsEnabled = !Successors.Any() && !ReturnDate.HasValue;

            TicketFreezes.ToList().ForEach(f => f.SerializedName = f.TicketFreezeReason.Name);

            //SolariumMinutesLeft
            SolariumMinutesLeft = SolariumMinutes;
            if (MinutesCharges.Count > 0)
            {
                SolariumMinutesLeft -= MinutesCharges.Sum(i => i.MinutesCharged);
            }

            SolariumMinutesFree = SolariumMinutes;
            if (SolariumVisits.Count > 0)
            {
                SolariumMinutesFree -= SolariumVisits.Where(i => i.Status == 0 || i.Status == 2).Sum(i => i.Amount);
            }

            InitActivity();
            ActivationEnabled = ActionsEnabled && Status == TicketStatus.Available;

            HasSuccessors = Successors.Any();

            //ResidualValue:
            if (UnitsAmount > 0)
            {
                var dayspercent = Length != 0 ? (decimal)LengthLeft / Length : 1;
                if (dayspercent >= Division.Company.ResidualValueP1) _residualValue = Cost / UnitsAmount * UnitsLeft * Division.Company.ResidualValueK11;
                else if (dayspercent >= Division.Company.ResidualValueP2) _residualValue = Cost / UnitsAmount * UnitsLeft * Division.Company.ResidualValueK12;
                else _residualValue = Cost / UnitsAmount * UnitsLeft * Division.Company.ResidualValueK13;
                _residualValue *= Division.Company.ResidualValueK2;
                _residualValue += Division.Company.ResidualValueS1;
                _residualValue -= Loan;
            }

            ReturnAmount = ResidualValue * (1 - Division.Company.TicketReturnPercentCommission) - Division.Company.TicketReturnFixedCommission;
            ReturnCommissionAmount = ResidualValue - ReturnAmount;
        }

        public void InitFinishDate()
        {
            if (StartDate.HasValue)
            {
                var freezes = 0;
                if (TicketFreezes.Count > 0)
                {
                    freezes = TicketFreezes.Where(t => t.TicketFreezeReasonId != Guid.Empty).Sum(f => f.Length);
                }
                FinishDate = StartDate.Value.AddDays(Length + freezes);
            }
        }


        public bool unitsOutInited = false;
        public void InitUnitsOut()
        {
            if (unitsOutInited) return;
            unitsOutInited = true;
            UnitsOut = UnitCharges.Sum(c => (int?)c.Charge) ?? 0;
            UnitsOutAuto = UnitCharges.Where(i => !i.EventId.HasValue).Sum(c => (int?)c.Charge) ?? 0;
            GuestUnitsOut = UnitCharges.Sum(c => (int?)c.GuestCharge) ?? 0;
        }

        public decimal ReturnCommissionAmount { get; private set; }

        private void InitActivity()
        {
            if ((FinishDate ?? DateTime.MaxValue) >= DateTime.Today
                && (UnitsAmount > 0 || SolariumMinutesLeft > 0)
                && (Loan == 0
                || (Loan > 0 && LastInstalmentDay.HasValue && LastInstalmentDay.Value > DateTime.Today)
                || (Cost > 0 && 1 - Loan / Cost >= Division.Company.ActivateInstalment / 100)
                || CreditInitialPayment.HasValue)
                && !ReturnDate.HasValue
                && Successors.Count == 0)
            {
                MayBeActive = true; //Надо добавить еще проверку на заморозки.
            }
        }

        void Ticket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DiscountPercent")
            {
                OnPropertyChanged("DiscountText");
                OnPropertyChanged("Cost");
                OnPropertyChanged("Loan");
            }

            if (e.PropertyName == "Price")
            {
                OnPropertyChanged("Cost");
                OnPropertyChanged("DiscountText");
            }
        }


        public TicketStatus Status
        {
            get
            {
                if (ReturnDate.HasValue) return TicketStatus.Returned;
                if (HasSuccessors) return TicketStatus.Rebilled;
                if (UnitsLeft <= 0 && SolariumMinutesLeft <= 0) return TicketStatus.RunOut;
                if (Loan > 0 &&
                    Cost != 0 &&
                    1 - (Loan / Cost) < SerializedActivateInstalment / 100 &&
                    ((LastInstalmentDay.HasValue && LastInstalmentDay.Value <= DateTime.Today)
                        || !LastInstalmentDay.HasValue)
                    && !CreditInitialPayment.HasValue) return TicketStatus.Unpaid;
                if (FinishDate.HasValue && FinishDate.Value < DateTime.Now) return TicketStatus.Expiried;
                if (TestFreeze()) return TicketStatus.Freezed;
                if (IsActive) return TicketStatus.Active;
                if (MayBeActive) return TicketStatus.Available;
                return TicketStatus.Unknown;
            }
        }

        public string StatusText
        {
            get
            {
                return GetStatusName(Status);
            }
        }

        private bool TestFreeze()
        {
            foreach (var freeze in SerializedTicketFreezes ?? TicketFreezes)
            {
                if (freeze.StartDate <= DateTime.Today && DateTime.Today <= freeze.FinishDate) return true;
            }
            return false;
        }

        public bool IsChangeEnabled
        {
            get
            {
                return (Status == TicketStatus.Active || Status == TicketStatus.Unpaid);
            }
        }

        public bool IsPassEnabled
        {
            get
            {
                return (Status == TicketStatus.Active);
            }
        }

        public bool IsFreezeEnabled
        {
            get
            {
                return (Status == TicketStatus.Active);
            }
        }

        public System.Windows.Visibility IsPaymentEnabled
        {
            get
            {
                return (Loan > 0 && !CreditInitialPayment.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public System.Windows.Visibility IsCreditPaymentEnabled
        {
            get
            {
                return (Loan > 0 && CreditInitialPayment.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        [DataMember]
        public ICollection<TicketPayment> SerializedTicketPayments { get; private set; }

        [DataMember]
        public ICollection<TicketFreeze> SerializedTicketFreezes { get; private set; }

        [DataMember]
        public bool HasSuccessors { get; private set; }

        /// <summary>
        /// Можно ли осуществлять возврат/замену/переоформление.
        /// На данный момент - факт наличия/отсутствия наследников
        /// </summary>
        [DataMember]
        public bool ActionsEnabled { get; private set; }

        [DataMember]
        public bool MayBeActive { get; private set; }

        [DataMember]
        public TicketType SerializedTicketType { get; set; }

        [DataMember]
        public Division SerializedDivision { get; private set; }

        [DataMember]
        public decimal GuestUnitsOut { get; set; }

        public decimal GuestUnitsLeft
        {
            get
            {
                return GuestUnitsAmount - GuestUnitsOut;
            }
        }

        public string DiscountText
        {
            get
            {
                return String.Format("{0:c} ({1:n0}%)", DiscountPercent * Price, DiscountPercent * 100);
            }
        }


        public decimal Cost
        {
            get
            {
                return Price * (1 - DiscountPercent);
            }
        }

        public decimal PaidAmount
        {
            get
            {
                if(SerializedTicketPayments != null && SerializedTicketPayments.Count > 0)
                {
                    return SerializedTicketPayments.Sum(p => p.Amount);
                }
                return 0;
            }
        }

        public decimal Loan
        {
            get
            {
                return Cost - PaidAmount;
            }
        }

        public decimal UnitPrice
        {
            get
            {
                if (UnitsAmount == 0) return 0;
                return Cost / UnitsAmount;
            }
        }

        [DataMember]
        public decimal ReturnAmount { get; private set; }

        public bool HasReturn
        {
            get
            {
                return ReturnDate.HasValue;
            }
        }

        [DataMember]
        public DateTime? FinishDate { get; set; }

        [DataMember]
        public decimal UnitsOutAuto { get; set; }

        [DataMember]
        public decimal UnitsOut { get; set; }

        public decimal UnitsLeft
        {
            get
            {
                return UnitsAmount - UnitsOut;
            }
        }

        public bool HasPayments
        {
            get
            {
                return SerializedTicketPayments != null && SerializedTicketPayments.Count > 0;
            }
        }

        public bool HasFreezes
        {
            get
            {
                return SerializedTicketFreezes != null && SerializedTicketFreezes.Count > 0;
            }
        }

        public int FreezesLeft
        {
            get
            {
                if (SerializedTicketFreezes.Count > 0)
                {
                    return FreezesAmount - (int)SerializedTicketFreezes.Sum(i => (i.FinishDate - i.StartDate).TotalDays + 1);
                }
                if (TicketFreezes.Count > 0)
                {
                    return FreezesAmount - (int)TicketFreezes.Sum(i => (i.FinishDate - i.StartDate).TotalDays + 1);
                }
                return FreezesAmount;
            }
        }

        public int LengthLeft
        {
            get
            {
                if (!StartDate.HasValue) return Length;
                var fr = 0;
                if (TicketFreezes != null && TicketFreezes.Count > 0)
                {
                    // Адский костыль:
                    // Т.к. расчет стоимости возврата зависит от заморозки, а при формировании возврата создается заморозка на 10 лет, то:
                    // делаем учет заморозки кроме 10 летней.
                    fr = TicketFreezes.ToList().Where(x => x.TicketFreezeReasonId != Guid.Empty).Sum(tf => tf.Length);
                }
                else if (SerializedTicketFreezes != null && SerializedTicketFreezes.Count > 0)
                {
                    fr = SerializedTicketFreezes.Where(x => x.TicketFreezeReasonId != Guid.Empty).Sum(tf => tf.Length);
                }
                return Length - (int)(DateTime.Today - StartDate.Value).TotalDays + fr;
            }
        }

        bool _helper;
        public bool Helper
        {
            get
            {
                return _helper;
            }
            set
            {
                _helper = value;
                OnPropertyChanged("Helper");
            }
        }

        private bool _activationEnabled;

        [DataMember]
        public bool ActivationEnabled
        {
            get { return _activationEnabled; }
            set
            {
                _activationEnabled = value;
                OnPropertyChanged("ActivationEnabled");
            }
        }

        public void RefreshStatus()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusText");
        }


        private decimal _residualValue;
        [DataMember]
        public decimal ResidualValue
        {
            get
            {
                return _residualValue;
            }
            private set
            {
                _residualValue = value;
                OnPropertyChanged("ResidualValue");
            }
        }

        public decimal FirstPayment
        {
            get
            {
                if (CreditInitialPayment.HasValue)
                {
                    return CreditInitialPayment.Value;
                }
                if (Instalment == null || Instalment.Id == Guid.Empty)
                {
                    return Cost;
                }
                if (Instalment.ContribAmount.HasValue) return Math.Min(Instalment.ContribAmount.Value, Cost);
                return Instalment.ContribPercent.Value * Cost;
            }
        }

        public string LoanText
        {
            get
            {
                if (Loan == 0) return String.Empty;
                return String.Format("{0:c} до {1:d}", Loan, LastInstalmentDay);
            }
        }

        public string InstalmentText
        {
            get
            {
                if (!InstalmentId.HasValue)
                {
                    return String.Format("{0:c} ({1}) оплачивается до {2:d}", Cost, RusCurrency.Str(Convert.ToDouble(Cost)), CreatedOn);
                }
                decimal contrib;
                decimal second;
                var third = 0m;

                if (Instalment.ContribAmount.HasValue)
                {
                    contrib = Instalment.ContribAmount.Value;
                }
                else
                {
                    contrib = Cost * Instalment.ContribPercent.Value;
                }

                if (Instalment.SecondPercent.HasValue)
                {
                    second = Instalment.SecondPercent.Value * Cost;
                    third = Cost - second - contrib;
                }
                else
                {
                    second = Cost - contrib;
                }

                var sb = new StringBuilder();
                sb.AppendFormat("{0:c} ({1}) оплачивается до {2:d}<br>{3:c} ({4}) оплачивается до {5:d}"
                    , contrib, RusCurrency.Str(Convert.ToDouble(contrib)), CreatedOn
                    , second, RusCurrency.Str(Convert.ToDouble(second)), CreatedOn.AddDays(Instalment.Length));
                if (third > 0)
                {
                    sb.AppendFormat("<br>{0:c} ({1}) оплачивается до {2:d}", third, RusCurrency.Str(Convert.ToDouble(third)), CreatedOn.AddDays(Instalment.Length + Instalment.SecondLength.Value));
                }
                return sb.ToString();
            }
        }

        [DataMember]
        public decimal SolariumMinutesLeft { get; set; }

        [DataMember]
        public decimal SerializedActivateInstalment { get; set; }

        [DataMember]
        public decimal SolariumMinutesFree { get; set; }

        public decimal VatCost
        {
            get
            {
                return Cost * (VatAmount ?? 0) / 100;
            }
        }

        public decimal CostWOVat
        {
            get
            {
                return Cost * (1 - (VatAmount ?? 0) / 100);
            }
        }

        internal static string GetStatusName(TicketStatus j)
        {
            if (j == TicketStatus.Returned) return Localization.Resources.Refunded;
            if (j == TicketStatus.Rebilled) return Localization.Resources.Rebilled;
            if (j == TicketStatus.RunOut) return Localization.Resources.RanOut;
            if (j == TicketStatus.Unpaid) return Localization.Resources.Unpaid;
            if (j == TicketStatus.Expiried) return Localization.Resources.Expired;
            if (j == TicketStatus.Freezed) return Localization.Resources.Freezed;
            if (j == TicketStatus.Active) return Localization.Resources.Active;
            if (j == TicketStatus.Available) return Localization.Resources.Available;
            return Localization.Resources.Unknown;
        }

        public DateTime? LoanPlanDate
        {
            get
            {
                //if (!InstalmentId.HasValue) return null;
                return PlanningInstalmentDay ?? LastInstalmentDay;
            }
        }

        public string LoanDetails
        {
            get
            {
                if (!CreditInitialPayment.HasValue) return String.Empty;
                var res = new StringBuilder();
                res.AppendFormat("Первый взнос по кредиту: {0:c0}", CreditInitialPayment);
                if (!String.IsNullOrWhiteSpace(CreditComment))
                {
                    res.AppendFormat(", комментарий: {0}", CreditComment);
                }
                return res.ToString();
            }
        }

        public decimal InitialInstallmentPercent
        {
            get
            {
                return (CreditInitialPayment ?? 0) / (Price * (1 - DiscountPercent));
            }
        }

        public string InitialInstallment
        {
            get
            {
                return String.Format("{0:c} ({1:p})", CreditInitialPayment, InitialInstallmentPercent);
            }
        }

        public void Init()
        {
            InitDetails();
        }

        public int SmartAmount
        {
            get
            {
                return (int)Math.Floor(UnitsAmount / 8);
            }
        }

        public int SmartOut
        {
            get
            {
                return (int)Math.Floor(UnitsOut / 8);
            }
        }

        public int SmartLeft
        {
            get
            {
                return SmartAmount - SmartOut;
            }
        }

       public int SmartGuestAmount
        {
            get
            {
                return (int)Math.Floor(GuestUnitsAmount / 8);
            }
        }

       public int SmartGuestOut
       {
           get
           {
               return (int)Math.Floor(GuestUnitsOut / 8);
           }
       }

       public int SmartGuestLeft
       {
           get
           {
               return SmartGuestAmount - SmartGuestOut;
           }
       }

        public string VisitStart
        {
            get
            {
                return TicketType.VisitStart.Substring(0, 2) + ":" + TicketType.VisitStart.Substring(2);
            }
        }
        public string VisitEnd
        {
            get
            {
                return TicketType.VisitEnd.Substring(0, 2) + ":" + TicketType.VisitEnd.Substring(2);
            }
        }
    }

    public enum TicketStatus : byte
    {
        Active = 1,
        Available = 2,
        Returned = 6,
        Rebilled = 5,
        Unpaid = 4,
        RunOut = 7,
        Expiried = 8,
        Freezed = 3,
        Unknown = 9
    }
}
