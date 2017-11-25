using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TonusClub.Infrastructure;
using TonusClub.ServiceModel;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Network.Windows
{
    public partial class NewEditTrgetTypeSetWindow
    {
        public ObservableCollection<TreatmentPlan> SelectedTreatments { get; set; }
        public Dictionary<Guid, string> Targets { get; set; }
        public Guid TargetId { get; set; }
        public Guid Id { get; set; }
        private TreatmentConfig[] _treatmentConfigs;

        public NewEditTrgetTypeSetWindow(ClientContext context, TargetTypeSet set)
            : base(context)
        {
            SelectedTreatments = new ObservableCollection<TreatmentPlan>();
            InitializeComponent();

            _treatmentConfigs = context.GetAllTreatmentConfigs().Where(i => i.IsActive).ToArray();

            if(set == null || set.Id == Guid.Empty)
            {
                Id = Guid.NewGuid();

                var tp = new TreatmentPlan { TreatmentConfigs = _treatmentConfigs };
                SelectedTreatments.Add(tp);
            }
            else
            {
                Id = set.Id;
                TargetId = set.TargetTypeId;
                (set.TreatmentConfigIds??"").Split(',').Where(i => !String.IsNullOrEmpty(i))
                    .Select(i => Guid.Parse(i))
                    .ToList().ForEach(i =>
                    {
                        var tpl = new TreatmentPlan { TreatmentConfigs = _treatmentConfigs};
                        tpl.TreatmentConfigId = i;
                        SelectedTreatments.Add(tpl);
                    });
            }


            Targets = context.GetCustomerTargetTypes().OrderBy(i=>i.Value).ToDictionary(i=>i.Key, i=>i.Value);

            var dm = new ListViewDragDropManager<TreatmentPlan>(listView)
            {
                ShowDragAdorner = true,
                DragAdornerOpacity = 0.75
            };


            DataContext = this;
        }

        private void TreatmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.RemovedItems.Count == 0)
            {
                var tp = new TreatmentPlan { TreatmentConfigs = _treatmentConfigs };
                SelectedTreatments.Add(tp);
            }
        }

        private void RemoveTreatmentPlanButton_Click(object sender, RoutedEventArgs e)
        {
            var tp = ((Button)sender).DataContext as TreatmentPlan;
            if(tp != null && tp.TreatmentConfigId == Guid.Empty) return;
            SelectedTreatments.Remove(tp);
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            _context.PostTargetSet(Id, TargetId, String.Join(",", SelectedTreatments.Where(i => i.TreatmentConfigId != Guid.Empty).Select(i => i.TreatmentConfigId.ToString())));
            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
