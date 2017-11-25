using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TonusClub.ServiceModel;

namespace TonusClub.ScheduleModule.Controls
{
    /// <summary>
    /// Interaction logic for TimelineControl.xaml
    /// </summary>
    public partial class TimelineControl : ContentControl
    {
        public TimelineControl()
        {
            InitializeComponent();
            IsTabStop = true;
            Focusable = true;
            DataContextChanged += new DependencyPropertyChangedEventHandler(TimelineControl_DataContextChanged);
        }

        void TimelineControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (_proposal != null)
                {
                    _proposal.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(_proposal_PropertyChanged);
                }
                _proposal = e.NewValue as ScheduleProposal;
                _proposal.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_proposal_PropertyChanged);
            }
        }

        void _proposal_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Prefer")
            {
                InvalidateVisual();
            }
        }

        private ScheduleProposal _proposal;
        public List<ScheduleProposalElement> Elements { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _proposal.Prefer = true;
            InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var w = RenderSize.Width;
            var h = RenderSize.Height;
            var span = (_proposal.MaxTime - _proposal.MinTime).TotalMinutes;
            var hrs = Math.Floor(span / 60);
            var minlen = w / span;
            var hrspan = (60 - _proposal.MinTime.Minute) * minlen;
            if (_proposal.MinTime.Minute == 0) hrspan = 0;

            foreach (var p in _proposal.List)
            {
                var st = (p.StartTime - _proposal.MinTime).TotalMinutes * minlen;
                var len = (p.EndTime - p.StartTime).TotalMinutes * minlen;

                if (new Rect(new Point(st, 0), new Size(len - 1, h - 1)).Contains(Mouse.GetPosition(this)))
                {
                    InvalidateVisual();
                    return;
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);


            //var newHeight = _proposal.GetMaxParallel() * 40;
            //if (newHeight != Height)
            //{
            //    Height = newHeight;
            //    return;
            //}

            var w = RenderSize.Width;
            var height = RenderSize.Height;
            dc.DrawRectangle(new SolidColorBrush(Colors.Transparent), new Pen(Brushes.Transparent, 0), new Rect(new Point(0, 0), new Size(w, height)));

            var span = (_proposal.MaxTime - _proposal.MinTime).TotalMinutes;
            var hrs = Math.Floor(span / 60);
            var minlen = w / span;
            var hrspan = (60 - _proposal.MinTime.Minute) * minlen;
            if (_proposal.MinTime.Minute == 0) hrspan = 0;


            if (_proposal.Prefer)
            {
                dc.DrawRectangle(Styles.SelectedTreatmentInnerBrush, Styles.SelectedTreatmentPen, new Rect(new Point(0, 0), new Size(w, height)));
            }

            Rect r1 = new Rect();
            FormattedText ft1 = null;
            var h2 = 0;
            var maxh1 = 40;
            for (int i = 0; i < _proposal.List.Count; i++)
            {
                var p = _proposal.List[i];
                var lineNum = 0;
                for (int j = 0; j < i; j++)
                {
                    if (ScheduleProposal.DatesIntersects(_proposal.List[i].StartTime, _proposal.List[i].EndTime, _proposal.List[j].StartTime, _proposal.List[j].EndTime))
                        lineNum++;
                }
                var h1 = 40 * lineNum;
                var st = (p.StartTime - _proposal.MinTime).TotalMinutes * minlen;
                var len = (p.EndTime - p.StartTime).TotalMinutes * minlen;
                var txt = String.Format("{0}\n{1:HH:mm}-{2:HH:mm}", p.Treatment.Tag, p.StartTime, p.EndTime);
                var ft = new FormattedText(txt, System.Globalization.CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, new Typeface(new FontFamily("Calibri"), FontStyles.Normal, FontWeights.UltraLight, FontStretches.Normal), 14, Styles.TextBrush);
                if (new Rect(new Point(st, h1), new Size(len - 1, 39)).Contains(Mouse.GetPosition(this)))
                {
                    ft.SetForegroundBrush(Brushes.White);
                    len = Math.Max(len, ft.Width + 3);
                    ft1 = ft;
                    h2 = h1;
                    r1 = new Rect(new Point(st, h1), new Size(len, 40));
                    continue;
                }
                if (p.MovedByRules)
                {
                    dc.DrawRectangle(Brushes.LightYellow, Styles.TreatmentBorderPen, new Rect(new Point(st + 1, h1 + 1), new Size(len - 2, 40 - 2)));
                }
                else
                {
                    dc.DrawRectangle(Styles.TreatmentInnerBrushHighlight, Styles.TreatmentBorderPen, new Rect(new Point(st + 1, h1 + 1), new Size(len - 2, 40 - 2)));
                }
                dc.PushClip(new RectangleGeometry(new Rect(new Point(st, h1), new Size(len - 1, 39))));
                if (_proposal.Prefer)
                {
                    ft.SetForegroundBrush(Styles.TextBrushSel);
                }
                dc.DrawText(ft, new Point(st + 2, 3 + h1));
                dc.Pop();
                maxh1 = Math.Max(h1+40, maxh1);
            }

            if (maxh1 > Height)
            {
                Height = maxh1;
                return;
            }

            for (int i = 0; i <= hrs; i++)
            {
                var x = (int)(i * minlen * 60 + hrspan);
                for (int j = 1; j < 6; j++)
                {
                    dc.DrawLine(Styles.MinutePen, new Point((int)(x + minlen * 10 * j), 0), new Point((int)(x + minlen * 10 * j), height));
                }
                dc.DrawLine(Styles.HourPen, new Point(x, 0), new Point(x, height));
            }

            if (ft1 != null)
            {
                dc.DrawRectangle(Styles.TreatmentInnerBrushHighlightSel, Styles.TreatmentBorderPenSel, r1);
                dc.PushClip(new RectangleGeometry(r1));
                dc.DrawText(ft1, new Point(r1.Left + 2, h2 + 3));
                dc.Pop();
            }
            dc.DrawLine(Styles.HorizontalPen, new Point(0, RenderSize.Height), new Point(RenderSize.Width, RenderSize.Height));
        }
    }
}