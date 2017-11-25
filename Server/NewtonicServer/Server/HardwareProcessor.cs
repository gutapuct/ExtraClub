using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace NewtonicServer.Server
{
    class HardwareProcessor
    {
        private TextProcessor text;

        Timer timer;

        List<ClientInfo> currentHw = new List<ClientInfo>();

        public event EventHandler<GuidEventArgs> Start;
        public event EventHandler<GuidEventArgs> Stop;

        protected void OnStart(Guid treatmentId)
        {
            if (Start != null) Start.Invoke(this, new GuidEventArgs { Id = treatmentId });
        }

        protected void OnStop(Guid treatmentId)
        {
            if (Stop != null) Stop.Invoke(this, new GuidEventArgs { Id = treatmentId });
        }

        public HardwareProcessor(TextProcessor textProc)
        {
            text = textProc;
            timer = new Timer(5000);
            timer.Elapsed += timer_Elapsed;

            timer.Start();
        }

        
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var cli in currentHw.ToArray())
            {
                if (cli.CurrentPlan != null)
                {
                    //check
                    var len = (cli.CurrentPlan.VisitDate.AddMinutes(cli.CurrentPlan.SerializedDuration) - DateTime.Now).TotalMinutes;
                    if (len <= 0)
                    {
                        StopTreatment(cli);
                    }
                    //text
                    else
                    {
                        text.SetText(cli, String.Format("Осталось {0:n0} мин", Math.Floor(len)+1), String.Format("Окончание: {0:H:mm}", cli.CurrentPlan.VisitDate.AddMinutes(cli.CurrentPlan.SerializedDuration)), 90);
                    }
                }
                if (cli.CurrentPlan == null)
                {
                    currentHw.Remove(cli);
                    text.SetText(cli, "Занятие окончено", "Ждем Вас снова!", 15);
                }
            }
        }

        internal void StartTreatment(ClientInfo cli, TonusClub.ServiceModel.TreatmentEvent plan)
        {
            if (cli.Treatment.MaxCustomers > 1)
            {
                currentHw.Where(i => i.Treatment != null
                        && i.Treatment.Id == cli.Treatment.Id
                        && i != cli
                        && i.CurrentPlan != null 
                        && i.CurrentPlan.CustomerId == plan.CustomerId)
                    .ToList().ForEach(i =>
                {
                    StopTreatment(i);
                });
            }
            cli.BlockCardInfo = new BlockCardInfo { CustomerId = plan.CustomerId, Timeout = plan.VisitDate.AddMinutes(plan.SerializedDuration + 3) };
            cli.CurrentPlan = plan;
            currentHw.Add(cli);
            OnStart(cli.HardwareId);
        }

        private void StopTreatment(ClientInfo i)
        {
            i.CurrentPlan = null;
            OnStop(i.HardwareId);
            currentHw.Remove(i);
        }
    }
}
