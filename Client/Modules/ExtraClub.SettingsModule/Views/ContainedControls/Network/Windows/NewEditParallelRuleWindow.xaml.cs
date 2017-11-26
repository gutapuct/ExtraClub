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
using System.ComponentModel;
using ExtraClub.ServiceModel;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.UIControls;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditParallelRuleWindow.xaml
    /// </summary>
    public partial class NewEditParallelRuleWindow
    {
        private IDictionaryManager _dictMan;

        public ICollectionView TreatmentTypesView1 { get; set; }
        public ICollectionView TreatmentTypesView2 { get; set; }

        private IEnumerable<TreatmentType> _treatmentTypes1;
        private IEnumerable<TreatmentType> _treatmentTypes2;

        public TreatmentsParalleling Rule { get; set; }
        TreatmentsParalleling oldRule;

        public NewEditParallelRuleWindow(IDictionaryManager dictMan, ClientContext context, TreatmentsParalleling rule)
        {
            this.DataContext = this;
            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            _treatmentTypes1 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            _treatmentTypes2 = context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();

            if (rule == null || rule.Type1Id == Guid.Empty)
            {
                Rule = new TreatmentsParalleling();
            }
            else
            {
                Rule = ViewModelBase.Clone<TreatmentsParalleling>(rule);
                oldRule = rule;
            }
            TreatmentTypesView1 = CollectionViewSource.GetDefaultView(_treatmentTypes1);
            TreatmentTypesView2 = CollectionViewSource.GetDefaultView(_treatmentTypes2);
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Rule.Type1Id == Guid.Empty) return;
            if (Rule.Type2Id == Guid.Empty) return;
            _context.PostTreatmentsParalleling(oldRule, Rule);
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
