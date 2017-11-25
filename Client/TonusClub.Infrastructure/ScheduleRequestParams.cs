using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.ServiceModel;

namespace TonusClub.Infrastructure
{
    public class ScheduleRequestParams
    {
        public Customer Customer { get; set; }
        public Action OnClose { get; set; }
        public DateTime? Date { get; set; }

        public Guid[] TreatmentConfigIds { get; set; }

        private bool _isCloseExecuted;
        public void ExecuteCloseMethod()
        {
            if(_isCloseExecuted) return;
            if(OnClose != null)
            {
                OnClose();
                _isCloseExecuted = true;
            }
        }
    }
}
