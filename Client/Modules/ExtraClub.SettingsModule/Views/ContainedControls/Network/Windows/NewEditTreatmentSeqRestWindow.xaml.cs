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
    public partial class NewEditTreatmentSeqRestWindow 
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView1 { get; set; }
        public ICollectionView TreatmentTypesView2 { get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes1;
        private IEnumerable<TreatmentType> _treatmentTypes2;

        public TreatmentSeqRest TreatmentSR { get; set; }

        public NewEditTreatmentSeqRestWindow(IDictionaryManager dictMan, ClientContext context, TreatmentSeqRest treatmentSR)
        {
            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            _treatmentTypes1 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            _treatmentTypes2 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (treatmentSR == null || treatmentSR.Id == Guid.Empty)
            {
                TreatmentSR = new TreatmentSeqRest();
            }
            else
            {
                TreatmentSR = treatmentSR;
            }
            TreatmentTypesView1 = CollectionViewSource.GetDefaultView(_treatmentTypes1);
            TreatmentTypesView2 = CollectionViewSource.GetDefaultView(_treatmentTypes2);
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentSR.TreatmentType1Id == Guid.Empty) return;
            if (TreatmentSR.TreatmentType2Id == null || TreatmentSR.TreatmentType2Id == Guid.Empty) return;
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
