using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ExtraClub.ScheduleModule.Controls
{
    internal static class Styles    
    {
        public static Pen HourPen = new Pen(new SolidColorBrush(Color.FromArgb(128, 0,0,25)), 1);
        public static Pen MinutePen = new Pen(new SolidColorBrush(Color.FromArgb(32, 0, 0, 25)), 1);
        public static Pen TreatmentBorderPen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#07508B")), 0.5);
        public static Brush TextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#07508B"));

        public static Brush TextBrushSel = new SolidColorBrush(Color.FromArgb(0xFF, 0x0D, 0x46, 0x63));

        public static Brush HourBrush = new SolidColorBrush(Colors.White);

        public static Brush SelectedTreatmentInnerBrush = new SolidColorBrush(Color.FromArgb(96, 255, 255, 255));

        public static Pen SelectedTreatmentPen = new Pen(SelectedTreatmentInnerBrush, 0.5);

        public static Pen HorizontalPen = new Pen(new SolidColorBrush(Color.FromArgb(128, 205, 205, 255)), 0.5);


        public static SolidColorBrush TreatmentInnerBrushHighlight = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));

        public static Pen TreatmentBorderPenSel = new Pen(new SolidColorBrush(Color.FromArgb(0,255,255,255)), 0.5);
        public static Brush TreatmentInnerBrushHighlightSel = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6BB1E4"));
    }
}
