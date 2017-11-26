using Microsoft.Practices.Unity;
using ExtraClub.UIControls.Interfaces;

namespace ExtraClub.Reports
{
    public class IoCModule
    {
        public void RegisterModule(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IReportManager, ReportManager>();
        }
    }
}
