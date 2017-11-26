using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using ExtraClub.SettingsModule.ViewModels;

namespace ExtraClub.SettingsModule.Views.ContainedControls.Franch
{
    public partial class FTargetsControl
    {
        public SettingsLargeViewModel Model => DataContext as SettingsLargeViewModel;

        public FTargetsControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fname = Path.Combine(Path.GetTempPath(),
                 $"Смарт-тренировки по целям_{DateTime.Now.ToString(CultureInfo.CurrentCulture).Replace(":", "").Replace("/", "")}.xlsx");

            var src = Model.TargetDetails.Where(d => !Model.IsClubTargetsOnly || (d.Item3)).ToList();
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Смарт-тренировки по целям");
            var i = 1;
            foreach (var pair in src)
            {
                ws.Range(i, 1, i, 5).Merge().SetValue(pair.Item1);
                i++;
                var j = 1;
                foreach (var plan in pair.Item2.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    ws.Cell(i, j++).SetValue(plan.Replace(", ", "\r\n").Replace("или ", ""));
                }
                i++;
            }
            ws.ColumnsUsed().AdjustToContents();
            wb.SaveAs(fname);
            Process.Start(fname);
        }

    }
}
