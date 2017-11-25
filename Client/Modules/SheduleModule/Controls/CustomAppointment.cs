using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls.ScheduleView;
using System.Windows.Media;

namespace TonusClub.ScheduleModule.Controls
{
    public class CustomAppointment : Appointment
    {
        public string CustomerName { get; set; }

        Brush _brush;
        public Brush CustomColor
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
                OnPropertyChanged("CustomColor");
            }
        }

        bool _SearchHighlight;
        public bool SearchHighlight
        {
            get
            {
                return _SearchHighlight;
            }
            set
            {
                    _SearchHighlight = value;
                    OnPropertyChanged("SearchHighlight");
                    OnPropertyChanged("SearchHighlightBrush");
                    OnPropertyChanged("ForeColor");
            }
        }

        public Brush SearchHighlightBrush
        {
            get
            {
                return _SearchHighlight ? Brushes.Red : Brushes.Transparent;
            }
        }

        public Brush ForeColor
        {
            get
            {
                return _SearchHighlight ? Brushes.White : Brushes.Black;
            }
        }
    }
}
