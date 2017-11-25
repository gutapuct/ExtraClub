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
using TonusClub.OrganizerModule.Views.Claims.Windows;
using TonusClub.OrganizerModule.ViewModels;
using TonusClub.ServiceModel;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.OrganizerModule.Views.Claims
{
    public partial class ClaimsControl
    {
        private OrganizerLargeViewModel Model
        {
            get
            {
                return DataContext as OrganizerLargeViewModel;
            }
        }

        public ClaimsControl()
        {
            InitializeComponent();
        }

        private void NewClaim_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserDialog<NewClaim>(() => Model.RefreshClaims());
        }

        private void ClaimsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var claim = ClaimsGrid.SelectedItem as Claim;
                if (claim.StatusId == -1)
                {
                    ProcessUserDialog<NewClaim>(() => Model.RefreshClaims(), new ParameterOverride("claim", claim));
                }
                else
                {
                    ProcessUserDialog<ClaimDetails>(() => Model.RefreshClaims(), new ParameterOverride("claim", claim));
                }
            }
        }

        private void ViewClaim_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedClaim == null) return;
            if (Model.SelectedClaim.StatusId == -1)
            {
                ProcessUserDialog<NewClaim>(() => Model.RefreshClaims(), new ParameterOverride("claim", Model.SelectedClaim));
            }
            else
            {
                ProcessUserDialog<ClaimDetails>(() => Model.RefreshClaims(), new ParameterOverride("claim", Model.SelectedClaim));
            }
        }

        private void NewClaimOnSelected_Click(object sender, RoutedEventArgs e)
        {
            var claim = ViewModelBase.Clone<Claim>(Model.SelectedClaim);
            claim.Id = Guid.Empty;
            claim.FtmId = null;
            claim.StatusId = 0;
            claim.CreatedOn = DateTime.Now;
            claim.CreatedBy = Model.ClientContext.CurrentUser.UserId;
            claim.FinishDate = null;
            claim.FinishDescription = null;
            claim.FinishedByFtmId = null;
            claim.FinishedByName = null;
            claim.StatusDescription = "Черновик";

            ProcessUserDialog<NewClaim>(() => Model.RefreshClaims(), new ParameterOverride("claim", claim));
        }
    }
}
