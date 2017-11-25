using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Infrastructure.BaseClasses;
using Microsoft.Practices.Unity;
using TonusClub.Infrastructure.Interfaces;
using System.ComponentModel;
using TonusClub.ServiceModel;
using System.Windows.Data;
using System.Threading;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.TreatmentsModule.ViewModels
{
    public class TreatmentsLargeViewModel : ViewModelBase
    {

        public ICollectionView ShelvesView { get; private set; }
        private List<CustomerShelf> _shelves = new List<CustomerShelf>();

        public ICollectionView SafesView { get; private set; }
        private List<CustomerShelf> _safes = new List<CustomerShelf>();

        public ICollectionView ChildrenView { get; private set; }
        private List<ChildrenRoom> _children = new List<ChildrenRoom>();

        public TreatmentsLargeViewModel(IUnityContainer container)
            : base()
        {
            ShelvesView = CollectionViewSource.GetDefaultView(_shelves);
            SafesView = CollectionViewSource.GetDefaultView(_safes);
            ChildrenView = CollectionViewSource.GetDefaultView(_children);
        }

        protected override void RefreshDataInternal()
        {
            _shelves.Clear();
            _shelves.AddRange(ClientContext.GetDivisionShelves(DateTime.Today.AddDays(-14), DateTime.Today, false));

            _safes.Clear();
            _safes.AddRange(ClientContext.GetDivisionShelves(DateTime.Today.AddDays(-14), DateTime.Today, true));

            _children.Clear();
            _children.AddRange(ClientContext.GetDivisionChildren(DateTime.Today.AddDays(-14), DateTime.Today));
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            ShelvesView.Refresh();
            SafesView.Refresh();
            ChildrenView.Refresh();
        }


    }
}
