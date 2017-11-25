using TonusClub.TurnoverModule.ViewModels;

namespace TonusClub.TurnoverModule.Views
{
    public partial class TurnoverAndProvidersControl
    {
        TurnoverLargeViewModel _model;

        public TurnoverAndProvidersControl(TurnoverLargeViewModel model)
        {
            InitializeComponent();
            DataContext = _model = model;
        }
    }
}
