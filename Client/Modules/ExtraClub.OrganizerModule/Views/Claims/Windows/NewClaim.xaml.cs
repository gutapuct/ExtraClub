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
using System.Windows.Shapes;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.OrganizerModule.Views.Claims.Windows
{
    public partial class NewClaim
    {
        #region DataContext

        public bool CanPublish
        {
            get
            {
                return ClaimTypeId.HasValue && !String.IsNullOrWhiteSpace(Claim.Subject) && String.IsNullOrWhiteSpace(Claim.Error);
            }
        }

        public Dictionary<int, string> ClaimTypes { get; set; }

        public List<Treatment> Treatments { get; set; }

        int? _claimTypeId;
        public int? ClaimTypeId
        {
            get
            {
                return _claimTypeId;
            }
            set
            {
                _claimTypeId = value;
                if (value.HasValue)
                {
                    Claim.ClaimTypeId = value.Value;
                }
                OnPropertyChanged("ClaimTypeId");
                OnPropertyChanged("CanPublish");
            }
        }

        public Claim Claim { get; set; }
        #endregion

        public NewClaim(ClientContext context, Claim claim)
            : base(context)
        {
            ClaimTypes = ClaimTypesClass.ClaimTypes;
            Treatments = context.GetAllTreatments();
            Treatments.Insert(0, new Treatment
            {
                Id = Guid.Empty,
                SerializedTreatmentType = new TreatmentType { Name = "Другое" }
            });
            InitializeComponent();
            if (claim.CompanyId == Guid.Empty)
            {
                Claim = new ServiceModel.Claim
                {
                    CompanyId = context.CurrentCompany.CompanyId,
                    ContactEmail = context.CurrentUser.Email,
                    ContactInfo = context.CurrentUser.FullName,
                    ContactPhone = context.CurrentCompany.Phone1
                };
            }
            else
            {
                Claim = claim;
            }
            Claim.FixCirculations();

            Claim.InitClaim();

            Claim.PropertyChanged += Claim_PropertyChanged;
            DataContext = this;
            ClaimTypeId = 0;
        }

        void Claim_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Eq_TreatmentId")
            {
                var t = Treatments.SingleOrDefault(i => i.Id == Claim.Eq_TreatmentId);
                if(t != null)
                {
                    Claim.Eq_Serial = t.SerialNumber;
                    Claim.Eq_BuyDate = t.Delivery;
                    Claim.Eq_Model = t.ModelName;
                }
                else
                {
                    Claim.Eq_Serial = String.Empty;
                    Claim.Eq_BuyDate = String.Empty;
                    Claim.Eq_Model = String.Empty;
                }
            }
            OnPropertyChanged("CanPublish");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Claim.PropertyChanged -= Claim_PropertyChanged;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!ClaimTypeId.HasValue)
            {
                return;
            }
            Claim.ClaimTypeId = ClaimTypeId.Value;
            if (String.IsNullOrWhiteSpace(Claim.Subject))
            {
                ExtraWindow.Alert("Ошибка", "Необходимо указать тему запроса!");
            }
            if (!String.IsNullOrWhiteSpace(Claim.Error)) return;
            Claim.StatusId = 0;
            _context.PostClaim(Claim);
            DialogResult = true;
            Claim.PropertyChanged -= Claim_PropertyChanged;
            Close();
        }

        private void Draft_Click(object sender, RoutedEventArgs e)
        {
            if (!ClaimTypeId.HasValue)
            {
                return;
            }
            Claim.ClaimTypeId = ClaimTypeId.Value;

            if (!String.IsNullOrWhiteSpace(Claim.Error)) return;
            Claim.StatusId = -1;
            _context.PostClaim(Claim);
            DialogResult = true;
            Claim.PropertyChanged -= Claim_PropertyChanged;
            Close();
        }
    }

    static class ClaimTypesClass
    {
        public static Dictionary<int, string> ClaimTypes { get; set; }

        static ClaimTypesClass()
        {
            ClaimTypes = new Dictionary<int, string>
            {
                {0,"Заявка в сервис (ремонт оборудования)"},
                { 2,"Служба поддержки сети"},
                {1,"Поддержка АСУ Extra Direction"},
                //{3,"Обратиться к учредителям сети"},
                //{13,"Согласовать рекламную акцию, макет"},
                //{20,"Заказать обучение, аттестацию"},
                //{23, "Расширение списка Smart-тренировок"},
                //{7,"Не работает почта, доступ к закрытому разделу"},
                //{17, "Доставка, отгрузка, транспортировка оборудования"},
                //{11,"Получить счёт"},
                //{15,"Прочее"},
                //{4,"Заявка на заказ смс-рассылки"},
                //{5,"Контекстная кампания в Яндекс-директ"},
                //{6,"Выставить счет на корпоративное издание"},
                //{8,"Заявка на вебинар"},
                //{9,"Заключение дилерского договора и размещение в интернет-магазине"},
                //{10,"Обновить страницы сайта"},
                //{12,"Выставить счет и договор на оборудование"},
                //{14,"Юридические вопросы"},
                //{16,"Вопросы по управлению клубом"},
                //{18,"Получитьконсультацию по АСУ ОнФлагмакс"},
                //{19,"Пообщаться с франчайзором"},
                //{21,"Бухгалтерия"},
                //{22,"Подтвердить участие в вебинаре"},

            };
        }
    }
}
