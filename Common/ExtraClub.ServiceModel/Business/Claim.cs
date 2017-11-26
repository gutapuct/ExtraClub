using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ExtraClub.ServiceModel
{
    /// <summary>
    /// Status:
    /// -1=Черновик
    /// 0=Ожидает отправки
    /// 1=ожидает синхронизации
    /// 2=размещена в фтм/возобновлена/на рассмотрении
    /// 3=выполняется в фтм
    /// 4=исполнена/подтвердите
    /// 5=закрыта
    /// 6=закрыта и синхронизирована
    /// 7=возобновлена
    /// </summary>
    partial class Claim : IDataErrorInfo
    {
        List<string> Circulations1, Circulations2;
        public List<string> Circulations { get; set; }

        public Claim()
        {
            InitClaim();
        }

        bool _isInit = false;
        public void InitClaim()
        {
            if (_isInit) return;
            PropertyChanged += Claim_PropertyChanged;

            Circulations1 = new List<string> { "500", "1000", "2000", "3000", "5000", "10000", "Более 10000" };
            Circulations2 = new List<string> { "500", "1000", "2000", "3000", "5000", "Более 5000" };

            FixCirculations();

            _isInit = true;
        }

        void Claim_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ClaimTypeId")
            {
                OnPropertyChanged("Hardware");
                OnPropertyChanged("Smart");
                OnPropertyChanged("Tirazh");
                FixCirculations();
            }
        }

        public void FixCirculations()
        {
            if (ClaimTypeId == 6)
            {
                Circulations = Circulations1;
                OnPropertyChanged("Circulations");
            }
            if (ClaimTypeId == 7)
            {
                Circulations = Circulations2;
                OnPropertyChanged("Circulations");
            }
        }

        public bool Hardware
        {
            get
            {
                return ClaimTypeId == 0;
            }
        }

        public bool Smart
        {
            get
            {
                return ClaimTypeId == 23;
            }
        }

        public bool Tirazh
        {
            get
            {
                return false;//ClaimTypeId == 6 || ClaimTypeId == 7;
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
            get
            {
                switch (columnName)
                {
                    case "ContactEmail":
                    case "ContactPhone":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(ContactEmail) || String.IsNullOrWhiteSpace(ContactPhone))
                            {
                                return "!";
                            }
                        }
                        else
                            if (String.IsNullOrWhiteSpace(ContactEmail) && String.IsNullOrWhiteSpace(ContactPhone))
                            {
                                return "!";
                            }
                        break;
                    case "Subject":
                        if (String.IsNullOrWhiteSpace(Subject))
                        {
                            return "!";
                        }
                        break;
                    case "ContactInfo":
                        if (String.IsNullOrWhiteSpace(ContactInfo))
                        {
                            return "!";
                        }
                        break;
                    case "Eq_TreatmentId":
                        if (ClaimTypeId == 0)
                        {
                            if (Eq_TreatmentId == null)
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Eq_BuyDate":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Eq_BuyDate))
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Eq_Serial":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Eq_Serial))
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Eq_Model":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Eq_Model))
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Eq_ClubAddr":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Eq_ClubAddr))
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Eq_PostAddr":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Eq_PostAddr))
                            {
                                return "!";
                            }
                        }
                        break;
                    case "Message":
                        if (ClaimTypeId == 0)
                        {
                            if (String.IsNullOrWhiteSpace(Message))
                            {
                                return "!";
                            }
                        }
                        break;
                }
                return null;
            }
        }
    }
}
