using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtraClub.Infrastructure.BaseClasses;
using ExtraClub.ServiceModel;
using ExtraClub.UIControls.BaseClasses;

namespace ExtraClub.OrganizerModule.ViewModels
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
