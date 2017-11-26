using System;
using System.Windows;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure.Interfaces;
using System.Globalization;
using System.Windows.Markup;
using System.IO;
using ExtraClub.WinClient.Windows;
using ExtraClub.Infrastructure;
using Telerik.Windows.Controls.External;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtraClub.UIControls;

namespace ExtraClub.WinClient
{
    public partial class App
    {
        private Bootstrapper _bootstrapper;

        private readonly Window _wnd = new Window();


        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                NavigationManager.CloseSplashRequest += NavigationManager_CloseSplashRequest;
                _wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _wnd.Width = 521;
                _wnd.Height = 263;
                _wnd.WindowStyle = WindowStyle.None;
                _wnd.AllowsTransparency = true;
                _wnd.ShowInTaskbar = false;
                _wnd.Topmost = true;
                var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraClub.WinClient.Images.SplashScreen1.png");

                if(stream != null)
                    _wnd.Background = new ImageBrush(BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad));

                _wnd.Show();




                //StyleManager.ApplicationTheme = new SummerThemeExternal();
                //new Test().ShowDialog();
                //Application.Current.Shutdown();
                //return;
                FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
                base.OnStartup(e);

                this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

                _bootstrapper = new Bootstrapper();
                StyleManager.ApplicationTheme = new SummerThemeExternal();

                CultureHelper.FixupCulture();

                _bootstrapper.Run();

                {
                    ((IInitializeable)_bootstrapper.Container.Resolve<IDictionaryManager>()).TryInitialize();
                    if(_bootstrapper.Shell != null)
                    {
                        ((Window)_bootstrapper.Shell).Show();

                        LocalizationManager.Manager = new ExtraClub.UIControls.Localization.CustomLocalizationManager();
                    }
                }

                _wnd.Close();

            }
            catch(BadImageFormatException ex)
            {
                Logger.Log(ex);
            }
        }

        void NavigationManager_CloseSplashRequest(object sender, EventArgs e)
        {
            try
            {
                _wnd.Close();
            }
            catch { }
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.StackTrace.Contains("DelegateHelper.OnGiveFeedbackEventHandler"))
            {
                e.Handled = true;
                return;
            }
            if (e.Exception.StackTrace.Contains("VerifyPathIsAnimatable"))
            {
                e.Handled = true;
                return;
            }
            new BugReportWindow(e.Exception).ShowDialog();
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_bootstrapper.Container.Resolve<ISettingsManager>().SaveAll();

            var di = new DirectoryInfo("temp");
            if (di.Exists)
            {
                foreach (var fi in di.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch { }
                }
            }

            base.OnExit(e);
        }
    }
}
