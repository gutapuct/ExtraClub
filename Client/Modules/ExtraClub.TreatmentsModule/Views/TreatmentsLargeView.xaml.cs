using System;
using ExtraClub.Infrastructure.Interfaces;
using ExtraClub.TreatmentsModule.ViewModels;
using ExtraClub.UIControls;
using Microsoft.Practices.Unity;
using ExtraClub.TreatmentsModule.Views.Windows;
using ExtraClub.Infrastructure;

namespace ExtraClub.TreatmentsModule.Views
{
    public partial class TreatmentsLargeView : ModuleViewBase, ILargeSection
    {
        TreatmentsLargeViewModel _model;

        public TreatmentsLargeView(TreatmentsLargeViewModel model)
        {
            this.DataContext = _model = model;
            InitializeComponent();
            model.UpdateFinished += model_UpdateFinished;
            NavigationManager.NewChildRequest += new EventHandler<ClientEventArgs>(navMan_NewChildRequest);
        }

        void navMan_NewChildRequest(object sender, ClientEventArgs e)
        {
            ApplicationDispatcher.UnityContainer.Resolve<NewChildRequestWindow>(new ResolverOverride[] { new ParameterOverride("customerId", e.ClientId) }).ShowDialog();
        }

        void model_UpdateFinished(object sender, EventArgs e)
        {
            _model.UpdateFinished -= model_UpdateFinished;

            if (_model.ClientContext.CurrentDivision != null && !_model.ClientContext.CurrentDivision.HasChildren)
            {
                MainTab.Items.Remove(ChildrenTab);
            }
        }

        public object GetTransferDataForMinimize()
        {
            return null;
        }

        public object GetTransferDataForRestore()
        {
            return null;
        }

        public override void SetState(object data)
        {
            base.SetState(data);
            _model.EnsureDataLoading();
            if (data is ResizeEventArgs)
            {
                if (((ResizeEventArgs)data).ActiveRegionName == "Nursery")
                {
                    MainTab.SelectedItem = ChildrenTab;
                }
                if (((ResizeEventArgs)data).ActiveRegionName == "Shelves")
                {
                    MainTab.SelectedItem = ShelvesTab;
                } 
                if (((ResizeEventArgs)data).ActiveRegionName == "Safe")
                {
                    MainTab.SelectedItem = SafeTab;
                }
            }

        }
    }
}
