using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UpdaterService
{
    public partial class FlagmaxUpdaterService : ServiceBase
    {
        readonly Timer _timer;
        CancellationTokenSource _cancelSource;
        Task _currentTask;

        public FlagmaxUpdaterService()
        {
            _cancelSource = new CancellationTokenSource();
            _timer = new Timer(180000);
            _timer.Elapsed += TimerElapsed;
            InitializeComponent();
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if(_currentTask != null)
            {
                return;
            }

            _currentTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    new UpdaterCore().Update(_cancelSource.Token);
                }
                catch(Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    _currentTask = null;
                }
            }, _cancelSource.Token);
        }

        protected override void OnStart(string[] args)
        {
            _cancelSource = new CancellationTokenSource();
            _timer.Start();
        }

        protected override void OnStop()
        {
            _cancelSource.Cancel();
            _timer.Stop();
            if(_currentTask != null)
            {
                _currentTask.Wait();
            }
        }
    }
}
