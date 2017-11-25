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
using System.Collections.ObjectModel;
using TonusClub.Infrastructure;
using Telerik.Windows.Controls;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentProgramWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentProgramWindow : TonusClub.UIControls.WindowBase
    {
        private IDictionaryManager _dictMan;

        public TreatmentProgram TreatmentProgram { get; set; }
 
        public List<CompanySettingsFolder> SettingsFolders { get; set; }

        public IEnumerable<TreatmentProgram> TreatmentPrograms { get; set; }

        public ObservableCollection<TreatmentPlan> SelectedTreatments { get; set; }
        public IEnumerable<TreatmentConfig> TreatmentConfigs { get; set; }

        public NewEditTreatmentProgramWindow(IDictionaryManager dictMan, ClientContext context, TreatmentProgram program):base(context)
        {
            SettingsFolders = context.GetCompanySettingsFolders(1);

            Owner = Application.Current.MainWindow;
            _dictMan = dictMan;

            TreatmentPrograms = _context.GetTreatmentPrograms();

            if (program == null || program.Id == Guid.Empty)
            {
                TreatmentProgram = new TreatmentProgram
                {
                };
            }
            else
            {
                TreatmentProgram = program;
            }

            SelectedTreatments = new ObservableCollection<TreatmentPlan>();
            TreatmentConfigs = context.GetAllTreatmentConfigs().Where(i=>i.IsActive).ToList();


            var tp = new TreatmentPlan { TreatmentConfigs = this.TreatmentConfigs };
            if (program.SerializedTreatmentProgramLines != null && program.SerializedTreatmentProgramLines.Count > 0)
            {
                foreach (var pl in program.SerializedTreatmentProgramLines)
                {
                    var tp1 = new TreatmentPlan { TreatmentConfigs = this.TreatmentConfigs };
                    SelectedTreatments.Add(tp1);
                    tp1.TreatmentConfigId = tp1.TreatmentConfigs.First(tt => tt.Id == pl.TreatmentConfigId).Id;
                }
            }
            SelectedTreatments.Add(tp);

            this.DataContext = this;

            InitializeComponent();

            var dragMgr = new ListViewDragDropManager<TreatmentPlan>(listView);
            dragMgr.ShowDragAdorner = true;
            dragMgr.DragAdornerOpacity = 0.75;

            listView.ItemsSource = SelectedTreatments;
            this.listView.DragEnter += OnListViewDragEnter;
        }

        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TreatmentProgram.Error)) return;
            var lines = new List<Guid>();
            SelectedTreatments.ToList().ForEach(i =>
            {
                if (i.TreatmentConfigId != Guid.Empty)
                {
                    lines.Add(i.TreatmentConfigId);
                }
            });
            var res = _context.PostTreatmentProgram(TreatmentProgram, lines);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreatmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0 && !SelectedTreatments.Any(i => i.TreatmentConfigId == Guid.Empty))
            {
                var tp = new TreatmentPlan { TreatmentConfigs = this.TreatmentConfigs };
                //tp.PropertyChanged += new PropertyChangedEventHandler(tp_PropertyChanged);
                SelectedTreatments.Add(tp);
                OnPropertyChanged("SelectedTreatments");
            }
        }


        private void RemoveTreatmentPlanButton_Click(object sender, RoutedEventArgs e)
        {
            var tp = ((RadButton)sender).DataContext as TreatmentPlan;
            if (tp.TreatmentConfigId == Guid.Empty) return;
            SelectedTreatments.Remove(tp);
        }
    }
}
