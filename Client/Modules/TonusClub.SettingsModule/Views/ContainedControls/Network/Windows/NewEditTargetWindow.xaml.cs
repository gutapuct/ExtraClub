using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class NewEditTargetWindow
    {
        public CustomerTargetType CustomerTargetType { get; set; }

        public List<TreatmentConfig> TreatmentConfigs { get; set; }


        public NewEditTargetWindow(ClientContext context, CustomerTargetType target)
            : base(context)
        {
            TreatmentConfigs = _context.GetAllTreatmentConfigsAdmin().Where(i => i.IsActive).ToList();

            if (target == null || target.Id == Guid.Empty)
            {
                CustomerTargetType = new CustomerTargetType
                {
                    IsAvail = true
                };
            }
            else
            {
                CustomerTargetType = target;
                foreach (var tt in TreatmentConfigs)
                {
                    if (CustomerTargetType.SerializedTreatmentConfigIds.Any(i => i == tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
            }
            InitializeComponent();

            DataContext = this;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostTargetType(CustomerTargetType, TreatmentConfigs.Where(i => i.Helper).Select(i => i.Id).ToArray());
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
