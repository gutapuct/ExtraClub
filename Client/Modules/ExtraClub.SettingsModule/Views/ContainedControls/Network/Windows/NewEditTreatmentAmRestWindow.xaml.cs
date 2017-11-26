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
using ExtraClub.Infrastructure.Interfaces;
using System.ComponentModel;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls;

namespace ExtraClub.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentAmRestWindow 
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView1 { get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes1;

        public TreatmentSeqRest TreatmentSR { get; set; }

        public NewEditTreatmentAmRestWindow(IDictionaryManager dictMan, ClientContext context, TreatmentSeqRest treatmentAR)
        {
            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            _treatmentTypes1 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (treatmentAR == null || treatmentAR.Id == Guid.Empty)
            {
                TreatmentSR = new TreatmentSeqRest
                {
                    Amount = 30
                };
            }
            else
            {
                TreatmentSR = treatmentAR;
            }
            TreatmentTypesView1 = CollectionViewSource.GetDefaultView(_treatmentTypes1);
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentSR.TreatmentType1Id == Guid.Empty) return;
            _context.PostTreatmentSeqRest(TreatmentSR);
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
