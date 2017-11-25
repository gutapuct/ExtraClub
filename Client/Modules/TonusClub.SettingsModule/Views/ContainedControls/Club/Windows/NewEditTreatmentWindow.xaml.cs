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
using Telerik.Windows.Controls;
using TonusClub.Infrastructure.Interfaces;
using System.ComponentModel;
using TonusClub.ServiceModel;

namespace TonusClub.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentWindow
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView{ get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes;

        public List<CompanySettingsFolder> SettingsFolders { get; set; }

        public Treatment Treatment { get; set; }

        public NewEditTreatmentWindow(IDictionaryManager dictMan, Treatment treatment)
        {
            SettingsFolders = _context.GetCompanySettingsFolders(3);

            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            _treatmentTypes = _context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (treatment == null || treatment.Id == Guid.Empty)
            {
                Treatment = new Treatment
                {
                    AuthorId = _context.CurrentUser.UserId,
                    CreatedOn = DateTime.Now,
                    DivisionId = _context.CurrentDivision.Id,
                    IsActive = true,
                    MaxCustomers = 1
                };
            }
            else
            {
                Treatment = treatment;
            }
            TreatmentTypesView = CollectionViewSource.GetDefaultView(_treatmentTypes);
            Treatment.PropertyChanged += new PropertyChangedEventHandler(Treatment_PropertyChanged);
            InitializeComponent();
            if (Treatment.Id != Guid.Empty)
            {
                type.IsEditable = false;
            }
            FixEnabled();
        }

        void Treatment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TreatmentTypeId" && Treatment.TreatmentTypeId != Guid.Empty)
            {
                FixEnabled();
            }
        }

        private void FixEnabled()
        {
            var tt = _treatmentTypes.FirstOrDefault(t => t.Id == Treatment.TreatmentTypeId);
            if (tt == null)
            {
                maxCustEdit.IsEnabled = false;
                Treatment.MaxCustomers = 1;
                return;
            }
            if (!(maxCustEdit.IsEnabled = tt.AllowsMultiple))
            {
                Treatment.MaxCustomers = 1;
            }
            durationText.Text = tt.Duration.ToString() + " мин.";
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Treatment.TreatmentTypeId == Guid.Empty) return;
            _context.PostTreatment(Treatment);
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
