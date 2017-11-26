using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ExtraClub.ServerCore;

namespace ExtraClub.WorkflowService.CommandLine
{
    internal sealed class Tools
    {
        public void Run(string[] args)
        {
            if (args[1] == "smart")
            {
                var dId = Guid.Parse(args[2]);
                var date = DateTime.Parse(args[3]);
                WorkflowCore.ChargeSmartTickets(dId, date);
                return;
            }

            if (args[1] == "workflow")
            {
                WorkflowCore.StartWorkflow(new CancelThreadInfo());
            }
        }
    }
}
