using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TonusClub.ServerCore;

namespace Flagmax.WorkflowService
{
    public partial class FlagmaxWorkflow : ServiceBase
    {
        static object _lock = new object();
        static object _lock2 = new object();

        CancelThreadInfo cancelInfo = new CancelThreadInfo();

        ManualResetEventSlim workflowState;

        public FlagmaxWorkflow()
        {
            this.ServiceName = "FlagmaxWorkflowService";
            InitializeComponent();

            workflowState = new ManualResetEventSlim(true);

        }

        private void ExecuteWorkflowTask(object state)
        {
            try
            {
                workflowWorker_DoWork(cancelInfo);
            }
            catch(Exception ex)
            {
                Log.WriteLine(DateTime.Now.ToString() + ": Работа обработчика СМС завершена c ошибкой");
                Logger.Log(ex);
            }
            lock(_lock)
            {
                workflowState.Set();
            }
        }

        System.Timers.Timer timer = new System.Timers.Timer(30000);

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            cancelInfo.Cancel = true;
        }

        protected override void OnPause()
        {
            base.OnPause();
            timer.Enabled = false;
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            timer.Enabled = true;
        }

        void workflowWorker_DoWork(CancelThreadInfo e)
        {
            //Log.WriteLine(DateTime.Now.ToString() + ": Обработчик запущен");
            //var sw = Stopwatch.StartNew();
            try
            {
                WorkflowCore.StartWorkflow(e);
            }
            catch(Exception ex)
            {
                while(ex != null)
                {
                    Log.WriteLine(DateTime.Now.ToString() + ": Работа обработчика завершена c ошибкой");
                    Log.WriteLine(ex.GetType().ToString());
                    Log.WriteLine(ex.Message);
                    Log.WriteLine(ex.StackTrace);
                    Log.WriteLine("\n\n");
                    ex = ex.InnerException;
                }
                return;
            }
            finally
            {
                //sw.Stop();
                //Log.WriteLine(DateTime.Now.ToString() + String.Format(": Работа обработчика завершена, время работы: {0}ms\n\n", sw.ElapsedMilliseconds));
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock(_lock)
            {
                if(workflowState.IsSet)
                {
                    workflowState.Reset();
                    ThreadPool.QueueUserWorkItem(ExecuteWorkflowTask);
                }
            }
        }
    }

}
