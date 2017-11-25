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
using TonusClub.Infrastructure.Interfaces;
using TonusClub.ServiceModel;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    public partial class NewEditTicketTypeWindow 
    {
        private IDictionaryManager _dictMan;

        public TicketType TicketType { get; set; }

        public IEnumerable<TreatmentType> TreatmentTypes { get; set; }
        public IEnumerable<CustomerCardType> CustomerCardTypes { get; set; }

        public List<SettingsFolder> SettingsFolders { get; set; }

        public List<LimitView> Limits { get; set; }

        public NewEditTicketTypeWindow(IDictionaryManager dictMan, ClientContext context, TicketType ticketType, bool readOnly)
            : base(context)
        {
            SettingsFolders = context.GetSettingsFolders(1, false);

            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            TreatmentTypes = _context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            CustomerCardTypes = _context.GetAllCustomerCardTypes().Where(i => i.IsActive).ToList();

            Limits = new List<LimitView>();

            var configs = context.GetAllTreatmentConfigs().Where(i => i.IsActive).ToList();

            if (ticketType == null || ticketType.Id == Guid.Empty)
            {
                TicketType = new TicketType
                {
                    ValidTo = DateTime.Today.AddYears(50),
                    FreezePriceCoeff = 1,
                    IsActive = true
                };
                configs.ToList().ForEach(i => Limits.Add(new LimitView { Id = i.Id, Name = i.Name, Amount = null }));
            }
            else
            {
                TicketType = ticketType;
                foreach (var tt in TreatmentTypes)
                {
                    if (TicketType.SerializedTreatmentTypes.Any(i => i.Id == tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
                foreach (var tt in CustomerCardTypes)
                {
                    if (TicketType.SerializedCustomerCardTypes.Any(i => i.Id == tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
                configs.ToList().ForEach(i => Limits.Add(new LimitView { Id = i.Id, Name = i.Name, Amount = GetLimitAmount(ticketType, i.Id) }));
            }

            InitializeComponent();

            if (!TicketType.IsActive)
            {
                RestoreButton.Visibility = System.Windows.Visibility.Visible;
                CommitButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (readOnly)
            {
                TicketType = ViewModelBase.Clone<TicketType>(TicketType);
                CommitButton.IsEnabled = false;
            }

            this.DataContext = this;

        }

        private string GetLimitAmount(TicketType ticketType, Guid configId)
        {
            var lim = ticketType.SerializedTicketTypeLimits.FirstOrDefault(i => i.TreatmentConfigId == configId);
            if (lim == null) return null;
            return lim.Amount.ToString();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TicketType.Error)) return;
            var res = _context.PostTicketType(TicketType);

            var tl = new List<Guid>();
            foreach (var i in TreatmentTypes)
            {
                if (i.Helper) tl.Add(i.Id);
            }

            _context.PostTicketTypeTreatmentTypes(res, tl);

            _context.PostTicketTypeCustomerCardTypes(res, CustomerCardTypes.Where(i => i.Helper).Select(i => i.Id).ToArray());

            var lims = new Dictionary<Guid, int>();
            foreach (var i in Limits)
            {
                int a;
                if (Int32.TryParse(i.Amount, out a))
                {
                    lims.Add(i.Id, a);
                }
            }

            _context.PostTicketTypeLimits(res, lims.ToArray());

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            _context.SetObjectActive("TicketTypes", TicketType.Id, true);
            DialogResult = true;
            Close();
        }
    }

    public class LimitView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
    }
}
