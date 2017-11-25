using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace TonusClub.ScheduleModule.Controls
{
    public class CustomStackPanel : StackPanel
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            //var w = RenderSize.Width;// -Padding.Left - Padding.Right;
            //var h = RenderSize.Height;
            //var sx = 0;//Padding.Left;

            //var span = (MaxTime - MinTime).TotalMinutes;
            //var hrs = Math.Floor(span / 60);
            //var minlen = w / span;
            //var hrspan = (60 - MinTime.Minute) * minlen;
            //if (MinTime.Minute == 0) hrspan = 0;

            //for (int i = 0; i <= hrs; i++)
            //{
            //    dc.DrawLine(Styles.HourPen, new Point(i * minlen * 60 + hrspan+sx, 0), new Point(i * minlen * 60 + hrspan+sx, h));
            //}

        }

        public DateTime MinTime { get; set; }

        public DateTime MaxTime { get; set; }
    }
}
