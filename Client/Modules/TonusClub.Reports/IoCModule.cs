using Microsoft.Practices.Unity;
using TonusClub.UIControls.Interfaces;

namespace TonusClub.Reports
{
    public class IoCModule
    {
        public void RegisterModule(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IReportManager, ReportManager>();
        }
    }
}
