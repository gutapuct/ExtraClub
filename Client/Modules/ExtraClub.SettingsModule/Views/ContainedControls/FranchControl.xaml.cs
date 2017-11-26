using ExtraClub.SettingsModule.ViewModels;
using ExtraClub.UIControls;

namespace ExtraClub.SettingsModule.Views.ContainedControls
{
    public partial class FranchControl
    {
        SettingsLargeViewModel _model;
        public FranchControl(SettingsLargeViewModel settingsModel)
        {
            DataContext = _model = settingsModel;
            InitializeComponent();

            //CompanySettingsTabControl.SelectedIndex = 9;
            //UMTabControl.SelectedIndex = 0;

            NavigationManager.ActivateLoginsRequest += NavigationManager_ActivateLoginsRequest;
        }

        void NavigationManager_ActivateLoginsRequest(object sender, ObjectEventArgs e)
        {
            CompanySettingsTabControl.SelectedIndex = 10;
            UMTabControl.SelectedIndex = 0;
        }
    }
}
