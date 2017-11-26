using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;
using ExtraClub.ScheduleModule.Views.Solarium;
using Telerik.Windows.Controls.ScheduleView;

namespace ExtraClub.ScheduleModule.Controls
{
    public class TreatmentDragBehavior : ScheduleViewDragDropBehavior
    {
        public override bool CanResize(DragDropState state)
        {
            return false;
        }

        public override bool CanDrop(DragDropState state)
        {
            foreach (var i in state.DestinationSlots)
            {
                if (SolariumGrid.LocateRes(i.Resources, "Treatment") != SolariumGrid.LocateRes(((Appointment)state.Appointment).Resources, "Treatment")) return false;
            }
            return base.CanDrop(state);
        }

        public override bool CanStartDrag(DragDropState state)
        {
            if (SolariumGrid.LocateRes(((Appointment)state.Appointment).Resources, "Status") != "0") return false;
            return base.CanStartDrag(state);
        }
    }
}
