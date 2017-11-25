using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Infrastructure.BaseClasses;
using TonusClub.ServiceModel;
using TonusClub.UIControls.BaseClasses;

namespace TonusClub.OrganizerModule.ViewModels
{
    public class NewsModel : ViewModelBase
    {
        public List<News> News { get; set; }

        public NewsModel(IUnityContainer cont)
            : base()
        {
        }

        protected override void RefreshDataInternal()
        {
            News = ClientContext.GetNews();
        }

        protected override void RefreshFinished()
        {
            base.RefreshFinished();
            OnPropertyChanged("News");
        }
    }
}
