using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;
using System.Windows;
using TonusClub.ScheduleModule.Controls;

namespace TonusClub.ScheduleModule.ViewModels
{
    public class MySpecialSlotStyleSelector : SpecialSlotStyleSelector
    {
        public Style MainAlternationHourStyle { get; set; }
        public Style AlternationMainHourStyle { get; set; }
        public Style AlternationAlternationHourStyle { get; set; }

        private Style _ReadonlyStyle;
        public Style ReadonlyStyle
        {
            get
            {
                return this._ReadonlyStyle;
            }
            set
            {
                this._ReadonlyStyle = value;
            }
        }

        public override Style SelectStyle(object item, DependencyObject container, ViewDefinitionBase activeViewDefinition)
        {
            if (item is MySlot)
            {
                var slot = item as MySlot;
                if (slot.MainAlternation && slot.SubAlternation)
                {
                    return AlternationAlternationHourStyle;
                }
                if (!slot.MainAlternation && slot.SubAlternation)
                {
                    return AlternationMainHourStyle;
                }
                if (slot.MainAlternation && !slot.SubAlternation)
                {
                    return MainAlternationHourStyle;
                }
            }

            if (item is Telerik.Windows.Controls.ScheduleView.Slot)
            {
                var slot = item as Telerik.Windows.Controls.ScheduleView.Slot;
                if (slot.IsReadOnly) return ReadonlyStyle;
            }

            return null;
        }
    }
}
