using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using System.ServiceModel;
using ExtraClub.UIControls;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    /// <summary>
    /// Interaction logic for NewEditTreatmentConfigWindow.xaml
    /// </summary>
    public partial class NewEditTreatmentConfigWindow : INotifyPropertyChanged
    {
        private IDictionaryManager _dictMan;


        public List<SettingsFolder> SettingsFolders { get; set; }

        public TreatmentConfig TreatmentConfig { get; set; }

        public List<TreatmentType> TreatmentTypes { get; set; }
        public ICollectionView TTView { get; set; }
        public TreatmentType SelectedTT
        {
            get
            {
                return TreatmentTypes.FirstOrDefault(i => i.Id == TreatmentConfig.TreatmentTypeId);
            }
        }

        public NewEditTreatmentConfigWindow(IDictionaryManager dictMan, ClientContext context, TreatmentConfig treatmentConfig)
        {
            SettingsFolders = context.GetSettingsFolders(3, false);
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            _dictMan = dictMan;

            TreatmentTypes = _context.GetAllTreatmentTypes().Where(i => i.IsActive).ToList();
            TTView = CollectionViewSource.GetDefaultView(TreatmentTypes);
            if (treatmentConfig == null || treatmentConfig.Id == Guid.Empty)
            {
                TreatmentConfig = new TreatmentConfig
                {
                    IsActive = true
                };
            }
            else
            {
                TreatmentConfig = treatmentConfig;
                typeBox.IsEnabled = false;
            }

            DataContext = this;

            TreatmentConfig.PropertyChanged += TreatmentConfig_PropertyChanged;
        }

        void TreatmentConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TreatmentTypeId")
            {
                OnPropertyChanged("SelectedTT");
            }
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TreatmentConfig.Error)) return;
            try
            {
                _context.PostTreatmentConfig(TreatmentConfig);
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
