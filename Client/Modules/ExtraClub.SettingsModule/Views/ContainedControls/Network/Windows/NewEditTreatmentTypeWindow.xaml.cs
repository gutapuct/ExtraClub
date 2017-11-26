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
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentTypeWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentTypeWindow
    {
        private IDictionaryManager _dictMan;

        public TreatmentType TreatmentType { get; set; }

        public IEnumerable<TreatmentTypeGroup> TreatmentTypeGroups { get; set; }

        public NewEditTreatmentTypeWindow(IDictionaryManager dictMan, ClientContext context, TreatmentType treatmentType)
        {
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            TreatmentTypeGroups = _context.GetAllTreatmentTypeGroups();

            if (treatmentType == null || treatmentType.Id == Guid.Empty)
            {
                TreatmentType = new TreatmentType
                {
                    IsActive = true
                };
            }
            else
            {
                TreatmentType = treatmentType;
            }

            this.DataContext = this;

            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TreatmentType.Error)) return;
            try
            {
                var res = _context.PostTreatmentType(TreatmentType);
            }
            catch (FaultException ex)
            {
                ExtraWindow.Alert("Ошибка", ex.Message);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
