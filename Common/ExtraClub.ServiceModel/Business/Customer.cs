using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;

namespace ExtraClub.ServiceModel
{
    partial class Customer: IComparable, IDataErrorInfo, IInitable
    {
        public Guid LikedTreatmentId { get; set; }
        public Guid LikedGoodId{ get; set; }
        public Guid LikedProgramId { get; set; }


        public string FullName
        {
            get
            {
                return (LastName ?? "") + ((" " + FirstName) ?? "") + ((" " + MiddleName) ?? "");
            }
        }

        partial void OnPropertyChangedInternal(string propertyName)
        {
            if (propertyName == "Phone1")
            {
                OnPropertyChanged("Phone1Valid");
            }
            if (propertyName == "Phone2")
            {
                OnPropertyChanged("Phone2Valid");
            }
            if (propertyName == "PasspNumber")
            {
                OnPropertyChanged("PasspNumberValid");
            }
            if (propertyName == "Email")
            {
                OnPropertyChanged("EmailValid");
            }
        }

        [DataMember]
        public CustomerCard ActiveCard { get; set; }

        [DataMember]
        public string PresenceStatusText { get; set; }

        [DataMember]
        public string SerializedStatus { get; private set; }

        [DataMember]
        public decimal CurrentBarDeposit { get; set; }

        /// <summary>
        /// Запускаться должно только на серверной стороне
        /// </summary>
        public void InitActiveCard()
        {
            try
            {
                if (CustomerCards.Count > 0)
                {
                    ActiveCard = CustomerCards.Where(i => i.IsActive).OrderByDescending(i => i.EmitDate).FirstOrDefault();
                    if (ActiveCard != null)
                    {
                        ActiveCard.InitDetails();
                    }
                }
            }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Запускаться должно только на серверной стороне
        /// </summary>
        public void InitDepositValues()
        {
            try
            {
                BonusDepositValue = this.BonusAccounts.Sum(a => a.Amount);
            }
            catch (InvalidOperationException) { BonusDepositValue = 0; }
            try
            {
                RurDepositValue = this.DepositAccounts.Sum(a => a.Amount);
            }
            catch (InvalidOperationException) { RurDepositValue = 0; }
        }

        //public void InitTickets()
        //{
        //    Tickets.ToList().ForEach(t => t.InitDetails());
        //    SerializedTickets = Tickets.ToList();
        //    SerializedTickets.ToList().ForEach(t => t.SerializedTicketType.Init());
        //}

        [DataMember]
        public decimal RurDepositValue { get; set; }

        [DataMember]
        public decimal BonusDepositValue { get; set; }

        //[DataMember]
        //public IList<Ticket> SerializedTickets { get; set; }

        [DataMember]
        public ICollection<CustomerCard> SerializedCustomerCards { get; set; }

        [DataMember]
        public string LikedTreatments { get; set; }
        [DataMember]
        public string LikedPrograms { get; set; }
        [DataMember]
        public string LikedGoods { get; set; }

        [DataMember]
        public string InvitorText { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        public void InitEssentials()
        {
            InitActiveCard();
            InitCustomerPresence();
            InitDepositValues();

            var cs = CustomerShelves.FirstOrDefault(i => !i.ReturnById.HasValue && !i.IsSafe);
            if (cs != null)
            {
                ShelfNumber = cs.ShelfNumber;
            }

            cs = CustomerShelves.FirstOrDefault(i => !i.ReturnById.HasValue && i.IsSafe);
            if (cs != null)
            {
                SafeNumber = cs.ShelfNumber;
            }
        }

        bool isInited = false;

        public void Init()
        {
            if (isInited) return;
            isInited = true;
            //InitTickets();
            //InitDepositValues();

            var res = from i in TreatmentEvents
                      where i.VisitStatus != (short)TreatmentEventStatus.Canceled
                      group i by i.TreatmentConfig.TreatmentType into j
                      select new { key = j.Key, value = j.Count() };
            var max = res.OrderByDescending(i=>i.value).FirstOrDefault();
            if (max != null && max.key != null)
            {
                LikedTreatmentId = max.key.Id;
                LikedTreatments = max.key.Name;
            }


            var res1 = from i in TreatmentEvents
                       where i.TreatmentProgram != null
                      group i by i.TreatmentProgram into j
                      select new { key = j.Key, value = j.Count() };
            var max1 = res1.OrderByDescending(i => i.value).FirstOrDefault();
            if (max1 != null && max1.key != null)
            {
                LikedProgramId = max1.key.Id;
                LikedPrograms= max1.key.ProgramName;
            }

            var dict = new Dictionary<Good, int>();
            foreach (var i in BarOrders)
            {
                foreach (var j in i.GoodSales)
                {
                    if (!dict.ContainsKey(j.Good)) dict.Add(j.Good, 0);
                    dict[j.Good] += (int)j.Amount;
                }
            }
            var goodpair = dict.OrderByDescending(i=>i.Value).FirstOrDefault();
            if (goodpair.Value != 0)
            {
                LikedGoodId = goodpair.Key.Id;
                LikedGoods = goodpair.Key.Name;
            }


            SerializedCustomerCards = CustomerCards;
            foreach (var card in SerializedCustomerCards)
            {
                card.InitDetails();
            }

            if(InvitorId.HasValue) {
                InvitedBy.InitEssentials();
                InvitorText = InvitedBy.FullName;
                if (InvitedBy.ActiveCard != null) InvitorText += ", карта номер " + InvitedBy.ActiveCard.CardBarcode;
            }

            InitStatus();
        }

        public void InitStatus()
        {
            SerializedStatus = String.Empty;

            if (CorporateId.HasValue)
            {
                SerializedStatus += "Корпоративный (" + Corporate.Name + "); ";
            }

            //if (CustomerVisits.Count < 10) SerializedStatus += "Новый клиент";
            //if (Tickets.Where(t => t.StartDate.HasValue).Count() > 2)
            //{
            //    var ts = Tickets.Where(t => t.StartDate.HasValue).OrderBy(t => t.StartDate).ToList();
            //    ts.ForEach(i => i.InitDetails());
            //    bool flag = true;
            //    for (int i = 1; i < ts.Count; i++)
            //    {
            //        if ((ts[i].StartDate.Value - ts[i - 1].FinishDate.Value).TotalDays > 90)
            //        {
            //            flag = false;
            //            break;
            //        }
            //    }
            //    if (flag)
            //    {
            //        if (!String.IsNullOrEmpty(SerializedStatus)) SerializedStatus += "; ";
            //        SerializedStatus += "Постоянный клиент";
            //    }
            //}
            foreach (var i in CustomerStatuses)
            {
                if (!String.IsNullOrEmpty(SerializedStatus)) SerializedStatus += "; ";
                SerializedStatus += i.Name;
            }
        }

        public void InitCustomerPresence()
        {
            var t = (from visit in CustomerVisits
                     where (visit.OutTime == null)
                     select visit).FirstOrDefault();
            if (t != null)
            {
                if (t.InTime.Date == DateTime.Today)
                {
                    PresenceStatusText = String.Format(Localization.Resources.CustomerHereTime, t.Division.Name, t.InTime);
                }
                else
                {
                    PresenceStatusText = String.Format(Localization.Resources.CustomerHereDate, t.Division.Name, t.InTime);
                }
                IsInClub = true;
            }


            if (String.IsNullOrEmpty(PresenceStatusText))
            {
                PresenceStatusText = Localization.Resources.CustomerNotInClub;
            }

        }

        [DataMember]
        public bool IsInClub { get; set; }

        public bool IsNotInClub
        {
            get { return !IsInClub; }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Customer)) return 0;
            return (FullName).CompareTo((obj as Customer).FullName);
        }

        public bool Phone1Valid
        {
            get
            {
                return ((Phone1 ?? "").ToCharArray().Count(n => n >= '0' && n <= '9') != 11);
            }
        }

        public bool Phone2Valid
        {
            get
            {
                var cnt = (Phone2 ?? "").ToCharArray().Count(n => n >= '0' && n <= '9');
                return  cnt != 11 && cnt != 12;
            }
        }

        public bool EmailValid
        {
            get
            {
                if (Email == null) return true;
                return !(Email.Contains('@') && Email.Contains('.') && Email.IndexOf('@') < Email.IndexOf('.'));
            }
        }

        public bool PasspNumberValid
        {
            get
            {
                var cnt = (PasspNumber ?? "").ToCharArray().Count(n => n >= '0' && n <= '9');
                return cnt != 10 && cnt != 7;
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!String.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        public string this[string columnName]
        {
            get {
                switch (columnName)
                {
                    case "LastName":
                    case "FirstName":
                    case "AdvertTypeId":
                        var pi = GetType().GetProperty(columnName);
                        if (pi == null) return null;
                        var obj = pi.GetValue(this, null);
                        if (obj == null || obj.Equals(String.Empty))
                            return "Обязательное поле";
                        break;
                    case "Birthday":
                        if (Birthday == null || Birthday < DateTime.Today.AddYears(-100)) return "Старый клиент";
                        break;
                    case "AdvertComment":
                        if (AdvertType == null) return null;
                        if (AdvertType.CommentNeeded && String.IsNullOrEmpty(AdvertComment)) return "Обязательное поле";
                        break;
                    case "Email":
                        if (HasEmail.GetValueOrDefault() && String.IsNullOrEmpty(Email))
                        {
                            return "Обязательное поле";
                        }
                        break;
                    case "InvitorId":
                        if (AdvertType == null) return null;
                        if (AdvertType.InvitorNeeded && !InvitorId.HasValue) return "Обязательное поле";
                        break;
                    case "PasspNumber":
                    case "PasspEmitPlace":
                        if (Thread.CurrentThread.CurrentUICulture.IetfLanguageTag.ToLower() != "ru-ru")
                        {
                            var pi1 = GetType().GetProperty(columnName);
                            if (pi1 == null) return null;
                            var obj1 = pi1.GetValue(this, null);
                            if (obj1 == null || obj1.Equals(String.Empty))
                            return "Required field";
                        }
                        break;
                    case "Phone2":
                        if (String.IsNullOrWhiteSpace(Phone2) || Phone2.Length < 5)
                        {
                            return "Необходимо указать телефон";
                        }
                        break;
                }
                return null;
            }
        }

        public string PassportData
        {
            get
            {
                return String.Format("№ {0}, выдан {1} {2:d}", PasspNumber, PasspEmitPlace, PasspEmitDate);
            }
        }

        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1} {2} {3}", AddrIndex, AddrCity, AddrStreet, AddrOther);
            }
        }

        [DataMember]
        public int? ShelfNumber { get; set; }

        [DataMember]
        public int? SafeNumber { get; set; }

        //public string TicketsText
        //{
        //    get
        //    {
        //        if (SerializedTickets==null || SerializedTickets.Count == 0) return String.Empty;
        //        var sb = new StringBuilder();
        //        foreach (var i in SerializedTickets)
        //        {
        //            if (sb.Length > 0) sb.Append("\n");
        //            sb.AppendFormat("№{3}, {0}, остаток единиц {1:n0}, истекает {2:d}", i.SerializedTicketType.Name, i.UnitsLeft, i.FinishDate, i.Number);
        //        }
        //        return sb.ToString();
        //    }
        //}

        public string ShortName
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(LastName);
                if (!String.IsNullOrWhiteSpace(FirstName))
                {
                    sb.AppendFormat(" {0}.", FirstName[0]);
                }
                if (!String.IsNullOrWhiteSpace(MiddleName))
                {
                    sb.AppendFormat(" {0}.", MiddleName[0]);
                }

                return sb.ToString();
            }
        }

        public string Phone2Formatted
        {
            get {
                if (Phone2 == null || Phone2.Length < 9) return Phone2;
                return "+" + Phone2.Substring(0, 1) + " (" + Phone2.Substring(1, 3) + ") " + Phone2.Substring(4, 3) + "-" + Phone2.Substring(7, 2) + "-" + Phone2.Substring(9);
            }
        }
    }
}
