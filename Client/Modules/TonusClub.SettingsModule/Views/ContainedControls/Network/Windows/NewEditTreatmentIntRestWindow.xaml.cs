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
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentIntRestWindow
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView1 { get; set; }
        public ICollectionView TreatmentTypesView2 { get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes1;
        private IEnumerable<TreatmentType> _treatmentTypes2;

        public TreatmentSeqRest TreatmentSR { get; set; }

        public NewEditTreatmentIntRestWindow(IDictionaryManager dictMan, ClientContext context, TreatmentSeqRest treatmentIR)
        {
            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            _treatmentTypes1 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            _treatmentTypes2 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (treatmentIR == null || treatmentIR.Id == Guid.Empty)
            {
                TreatmentSR = new TreatmentSeqRest
                {
                    Interval = 15
                };
            }
            else
            {
                TreatmentSR = treatmentIR;
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
