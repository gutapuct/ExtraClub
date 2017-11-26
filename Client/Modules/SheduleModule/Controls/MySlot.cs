using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.ScheduleView;

namespace ExtraClub.ScheduleModule.Controls
{
    public class MySlot : Slot
    {
        public bool MainAlternation { get; set; }
        public bool SubAlternation { get; set; }

        public override Slot Copy()
        {
            Slot slot = new MySlot
            {
                Start = Start,
                End = End,
            };

            slot.CopyFrom(this);
            return slot;
        }

        public override void CopyFrom(Slot other)
        {
            var otherSlot = other as MySlot;
            if (otherSlot != null)
            {
                this.MainAlternation = otherSlot.MainAlternation;
                this.SubAlternation = otherSlot.SubAlternation;
                base.CopyFrom(otherSlot);
            }
        }
    }
}
