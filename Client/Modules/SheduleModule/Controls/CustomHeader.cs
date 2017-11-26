using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ExtraClub.ScheduleModule.Controls
{
    public class CustomHeader : Border
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            var w = (ClientWidth > 0) ? ClientWidth : RenderSize.Width;
            var h = RenderSize.Height;
            var sx = 0;

            dc.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), RenderSize)));

            var span = (MaxTime - MinTime).TotalMinutes;
            var hrs = Math.Floor(span / 60);
            var minlen = w / span;
            var hrspan = (60 - MinTime.Minute) * minlen;
            if (MinTime.Minute == 0) hrspan = 0;

            for (int i = 0; i <= hrs; i++)
            {
                var x = (int)(i * minlen * 60 + hrspan + sx);
                for (int j = 1; j < 6; j++)
                {
                    dc.DrawLine(Styles.MinutePen, new Point((int)(x + minlen * 10 * j), 0), new Point((int)(x + minlen * 10 * j), h));
                }
                dc.DrawLine(Styles.HourPen, new Point(x, 0), new Point(x, h));
                dc.DrawText(new FormattedText(((MinTime.Hour + i)%24).ToString()+":00", System.Globalization.CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), 14, Styles.HourBrush), new Point(x + 3, 0));
            }
            base.OnRender(dc);
        }


        public DateTime MinTime { get; set; }

        public DateTime MaxTime { get; set; }



        public double ClientWidth { get; set; }
    }
}
