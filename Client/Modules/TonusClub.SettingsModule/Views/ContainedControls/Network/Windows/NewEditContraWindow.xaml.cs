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
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditContraWindow.xaml
    /// </summary>
    public partial class NewEditContraWindow
    {
        private IDictionaryManager _dictMan;

        public ContraIndication ContraIndication { get; set; }

        public IEnumerable<TreatmentType> TreatmentTypes { get; set; }


        public NewEditContraWindow(IDictionaryManager dictMan, ClientContext context, ContraIndication contraInd)
        {
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            TreatmentTypes = _context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (contraInd == null || contraInd.Id == Guid.Empty)
            {
                ContraIndication = new ContraIndication
                {
                    IsVisible = true
                };
            }
            else
            {
                ContraIndication = contraInd;
                foreach (var tt in TreatmentTypes)
                {
                    if (ContraIndication.SerializedTreatmentTypes.Any(i => i.Id == tt.Id))
                    {
                        tt.Helper = true;
                    }
                }
            }

            this.DataContext = this;

            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ContraIndication.Name)) return;
            var res = _context.PostContraIndication(ContraIndication);

            var tl = new List<Guid>();
            foreach (var i in TreatmentTypes)
            {
                if (i.Helper) tl.Add(i.Id);
            }

            _context.PostContraIndicationTreatmentTypes(res, tl);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
