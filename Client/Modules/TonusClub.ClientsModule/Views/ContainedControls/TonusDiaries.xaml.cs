using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ClosedXML.Excel;
using TonusClub.Clients.ViewModels;
using Microsoft.Practices.Unity;
using TonusClub.UIControls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using TonusClub.ServiceModel;
using TonusClub.UIControls.Windows;

namespace TonusClub.Clients.Views.ContainedControls
{
    public partial class TonusDiaries : ModuleViewBase
    {
        private ClientLargeViewModel model
        {
            get
            {
                return DataContext as ClientLargeViewModel;
            }
        }

        public RedLetterDayConverter RedConverter { get; set; }

        public TonusDiaries()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(TonusDiaries_DataContextChanged);
        }

        void TonusDiaries_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(model != null)
            {
                model.ClientSelected += new EventHandler(model_ClientSelected);
            }
        }

        void model_ClientSelected(object sender, EventArgs e)
        {
            var i = VisitCalendar.CalendarDayButtonStyle;
            VisitCalendar.CalendarDayButtonStyle = null;
            VisitCalendar.CalendarDayButtonStyle = i;
        }

        private void NoContrasButton_Click(object sender, RoutedEventArgs e)
        {
            model.SetNoContras();
        }

        private void SaveContrasButton_Click(object sender, RoutedEventArgs e)
        {
            model.SaveContras();
        }

        private void TargetsViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if(originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if(row != null)
                {
                    EditTargetButton_Click(null, null);
                }
            }
        }

        private void NewTargetButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
                return;
            ProcessUserDialog<NewEditTargetWindow>(() => model.RefreshTargets(), new ResolverOverride[] { new ParameterOverride("customer", model.CurrentCustomer) });

        }

        private void EditTargetButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedTarget != null && model.CurrentCustomer != null && !model.SelectedTarget.TargetComplete.HasValue)
            {
                ProcessUserDialog<NewEditTargetWindow>(() => model.RefreshTargets(), new ResolverOverride[] { new ParameterOverride("target", model.SelectedTarget), new ParameterOverride("customer", model.CurrentCustomer) });
            }
        }

        private void DeleteTargetButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedTarget != null && model.CurrentCustomer != null)
            {
                TonusWindow.Confirm("Удаление",
                                "Удалить цель \"" + model.SelectedTarget.SerializedTypeName + "\"?",
                                w =>
                                {
                                    if((w.DialogResult ?? false))
                                    {
                                        ClientContext.DeleteObject("CustomerTargets", model.SelectedTarget.Id);
                                        model.RefreshTargets();
                                    }
                                });
            }
        }

        private void PrintGoalsButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
            {
                return;
            }

            GoalsButton.IsEnabled = false;
            try
            {
                var cutomerId = model.CurrentCustomer.Id;
                var t = ClientContext.ExecuteMethodAsync(ts => ts.GetTreatmentTypesForCustomerGoals(ClientContext.CurrentDivision.Id, cutomerId));
                t.ContinueWith(i =>
            {
                try
                {
                    var sw = new StringWriter();

                    sw.WriteLine("<h2>Ваши цели:</h2>");
                    foreach(var item in t.Result.GroupBy(j => j.Item1))
                    {
                        sw.WriteLine($"<h4>{item.Key}</h4>");
                        foreach(var j in item)
                        {
                            sw.WriteLine("<ul>");
                            foreach(var tt in j.Item2)
                            {
                                sw.WriteLine($"<li>{tt}</li>");
                            }
                            sw.WriteLine("</ul>");
                            sw.WriteLine("<hr/>");
                        }
                    }

                    ReportManager.PrintHtmlToPdf(sw.ToString());
                }
                finally
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        GoalsButton.IsEnabled = true;
                    }));
                }
            });
            }
            catch
            {
                GoalsButton.IsEnabled = true;
                throw;
            }
        }

        //private void CancelTargetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (model.SelectedTarget == null || model.SelectedTarget.TargetComplete.HasValue) return;
        //    TonusWindow.Confirm(new DialogParameters
        //    {
        //        Header = "Удаление",
        //        Content = "Удалить цель \"" + model.SelectedTarget.TargetText + "\"?",
        //        OkButtonContent = "Да",
        //        CancelButtonContent = "Нет",
        //        Closed = delegate(object sender1, WindowClosedEventArgs e1)
        //        {
        //            if ((e1.DialogResult ?? false))
        //            {
        //                ClientContext.DeleteObject("CustomerTargets", model.SelectedTarget.Id);
        //                model.RefreshTargets();
        //            }
        //        },
        //        Owner = Application.Current.MainWindow
        //    });
        //}


        private void AnthrosViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if(originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if(row != null)
                {
                    EditAnthroButton_Click(null, null);
                }
            }
        }

        private void NewAnthroButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
                return;
            ProcessUserDialog<NewEditAnthropometricWindow>(() => model.RefreshAnthros(), new ResolverOverride[] { new ParameterOverride("customer", model.CurrentCustomer) });
        }

        private void EditAnthroButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedAnthro != null && model.CurrentCustomer != null)
            {
                ProcessUserDialog<NewEditAnthropometricWindow>(() => model.RefreshAnthros(), new ResolverOverride[] { new ParameterOverride("anthro", model.SelectedAnthro), new ParameterOverride("customer", model.CurrentCustomer) });
            }
        }

        private void DeleteAnthroButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedAnthro == null)
                return;
            TonusWindow.Confirm("Удаление",
                 "Удалить показатели за " + model.SelectedAnthro.CreatedOn.ToString("dd.MM.yyyy") + "?",
                    e1 =>
                    {
                        if((e1.DialogResult ?? false))
                        {
                            ClientContext.DeleteObject("Anthropometrics", model.SelectedAnthro.Id);
                            model.RefreshAnthros();
                        }
                    });
        }

        private void ExportAnthroButton_Click(object sender, RoutedEventArgs e)
        {
            if (model._Anthros.Count <= 1)
            {
                TonusWindow.Alert("Отчет по клиенту", "Отчет по клиенту нельзя построить без измерений");
                return;
            }
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            var fname = Path.Combine(Path.GetTempPath(),
               $"Отчет по клиенту_{DateTime.Now.ToString(CultureInfo.CurrentCulture).Replace(":", "").Replace("/", "")}.xlsx");
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Отчет по клиенту");

            #region шапка

            var columnsCount = 1 + 1 + (model._Anthros.Count * 2 - 1) + 1;
            ws.Cell(1, 1).InsertData(new string[] { "Отчет по клиенту" });
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontName = "Arial Black";
            ws.Range(1, 1, 1, columnsCount).Merge();

            var client = new List<string[]>() { new[] { "Клиентка:", model.CurrentCustomer.FullName } };
            var reportRow = new List<string[]>() { new[] { "Отчет  на дату:", DateTime.Today.ToShortDateString() } };
            ws.Cell(2, 1).InsertData(client);
            ws.Cell(3, 1).InsertData(reportRow);
            ws.Cell(2, 1).Style.Font.Bold = true;
            ws.Cell(3, 1).Style.Font.Bold = true;
            #endregion

            #region заголовок таблицы

            var headerFirstRowIndex = 4;
            var headerFirstColumnIndex = 1;
            var header = new List<string[]>();

            var sortedAnthros = model._Anthros.OrderBy(a => a.CreatedOn).ToList();

            var firstHeaderRow = new List<string>(columnsCount);
            var secondHeaderRow = new List<string>(columnsCount);

            firstHeaderRow.AddRange(new[] { "Показатели", "Норма по анализатору", "1 замер" });
            secondHeaderRow.AddRange(new[] { string.Empty, string.Empty, sortedAnthros[0].CreatedOn.ToShortDateString() });

            for (int i = 1; i < sortedAnthros.Count; i++)
            {
                firstHeaderRow.AddRange(new[] { $"{i + 1} замер", "Изменение" });
                secondHeaderRow.AddRange(new[] { sortedAnthros[i].CreatedOn.ToShortDateString(), string.Empty });
            }
            firstHeaderRow.Add("Изменение по нарастающим");
            secondHeaderRow.Add(string.Empty);

            header.Add(firstHeaderRow.ToArray());
            header.Add(secondHeaderRow.ToArray());

            var headerCell = ws.Cell(headerFirstRowIndex, headerFirstColumnIndex);
            headerCell.InsertData(header);
            headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerCell.Style.Font.Bold = true;
            ws.Range(headerFirstRowIndex, headerFirstColumnIndex, headerFirstRowIndex + 1, headerFirstColumnIndex).Merge();

            #endregion

            #region данные измерений

            var anthrosContentFirstRowIndex = headerFirstRowIndex + header.Count;
            var anthrosContentFirstColumnIndex = headerFirstColumnIndex;

            var anthrosToExport = new List<AnthropometricToExport>
            {
                new AnthropometricToExport(UIControls.Localization.Resources.Weight, i => sortedAnthros[i].Weight, a => !string.IsNullOrWhiteSpace(a.Weight)),
                new AnthropometricToExport(UIControls.Localization.Resources.Tall, i => sortedAnthros[i].Height, a => !string.IsNullOrWhiteSpace(a.Height)),
                new AnthropometricToExport(UIControls.Localization.Resources.ChestIn, i => sortedAnthros[i].ChestIn, a => !string.IsNullOrWhiteSpace(a.ChestIn)),
                new AnthropometricToExport(UIControls.Localization.Resources.ChestOut, i => sortedAnthros[i].ChestOut, a => !string.IsNullOrWhiteSpace(a.ChestOut)),
                new AnthropometricToExport(UIControls.Localization.Resources.Waist, i => sortedAnthros[i].Waist, a => !string.IsNullOrWhiteSpace(a.Waist)),
                new AnthropometricToExport(UIControls.Localization.Resources.Stomach, i => sortedAnthros[i].Stomach, a => !string.IsNullOrWhiteSpace(a.Stomach)),
                new AnthropometricToExport(UIControls.Localization.Resources.Hips, i => sortedAnthros[i].Leg, a => !string.IsNullOrWhiteSpace(a.Leg)),
                new AnthropometricToExport(UIControls.Localization.Resources.Buttocks, i => sortedAnthros[i].Buttocks, a => !string.IsNullOrWhiteSpace(a.Buttocks)),
                new AnthropometricToExport(UIControls.Localization.Resources.Fats, i => sortedAnthros[i].Fat, a => !string.IsNullOrWhiteSpace(a.Fat)),
                new AnthropometricToExport(UIControls.Localization.Resources.PSPulse, i => sortedAnthros[i].PSPulse, a => !string.IsNullOrWhiteSpace(a.PSPulse)),
                new AnthropometricToExport(UIControls.Localization.Resources.MusculeMass, i => sortedAnthros[i].MusculeMass, a => !string.IsNullOrWhiteSpace(a.MusculeMass)),
            };

            var anthrosContent = anthrosToExport.Select(row => GetRowWithMeasure(sortedAnthros, row.Title, row.GetValueByIndex.Value, row.Match.Value).ToArray()).ToList();

            ws.Cell(anthrosContentFirstRowIndex, anthrosContentFirstColumnIndex).InsertData(anthrosContent);

            #endregion

            #region тренировки

            var treatmentsFirstRowIndex = anthrosContentFirstRowIndex + anthrosContent.Count + 1;
            var treatmentsFirstColumnIndex = headerFirstColumnIndex;
            var treatmentData = new List<string[]>();
            var treatmentsHeaderRow = new List<string>(columnsCount);

            treatmentsHeaderRow.AddRange(new[] { "Количество процедур за период", string.Empty, string.Empty });
            var visitedTraitmentEvents = ClientContext.GetCustomerEvents(model.CurrentCustomer.Id, sortedAnthros[0].CreatedOn, sortedAnthros.Last().CreatedOn, false)
                .Where(r => r.VisitStatus == 2).ToList();
            var traitmentsNames = visitedTraitmentEvents.Select(r => r.SerializedTreatmentTypeName).Distinct();
            var traitmentsNamesDictionary = traitmentsNames.ToDictionary(traitmentsName => traitmentsName, traitmentsName => new List<string>());

            var startPeriodAnthros = sortedAnthros[0];
            for (var i = 1; i < sortedAnthros.Count; i++)
            {
                var endPeriodIndex = i;
                var anthros = startPeriodAnthros;
                var treatmentEventsByPeriod = visitedTraitmentEvents.Where(t => t.VisitDate < sortedAnthros[endPeriodIndex].CreatedOn.Date &&
                            t.VisitDate > anthros.CreatedOn.Date).ToList();

                foreach (var trait in traitmentsNamesDictionary)
                {
                    var treatmentCount = treatmentEventsByPeriod.Count(r => r.SerializedTreatmentTypeName == trait.Key);
                    trait.Value.AddRange(treatmentCount > 0
                        ? new[] { treatmentCount.ToString(), string.Empty }
                        : new[] { string.Empty, string.Empty });
                }

                treatmentsHeaderRow.AddRange(new[] { treatmentEventsByPeriod.Count().ToString(), string.Empty });
                startPeriodAnthros = sortedAnthros[endPeriodIndex];
            }
            treatmentData.Add(treatmentsHeaderRow.ToArray());
            foreach (var stringse in traitmentsNamesDictionary)
            {
                var row = new List<string> { stringse.Key, string.Empty, string.Empty };
                row.AddRange(stringse.Value);
                treatmentData.Add(row.ToArray());
            }

            ws.Cell(treatmentsFirstRowIndex, treatmentsFirstColumnIndex).InsertData(treatmentData);

            #endregion

            var recomendationRowIndex = treatmentsFirstRowIndex + treatmentData.Count + 2;
            var recomendationColumnIndex = treatmentsFirstColumnIndex;

            ws.Cell(recomendationRowIndex, recomendationColumnIndex).InsertData(new[] { "Рекомендации инструктора:" });
            ws.Cell(recomendationRowIndex, recomendationColumnIndex).Style.Font.Bold = true;

            try
            {
                ws.Range(anthrosContentFirstRowIndex,
                        anthrosContentFirstColumnIndex + 2,
                        anthrosContentFirstRowIndex + anthrosContent.Count,
                        firstHeaderRow.Count)
                    .DataType = XLCellValues.Number;
            }
            catch (Exception)
            {
                // кривые данные :(
            }

            ws.Range(treatmentsFirstRowIndex,
                    treatmentsFirstColumnIndex + 2,
                    treatmentsFirstRowIndex + treatmentData.Count,
                    firstHeaderRow.Count)
                .DataType = XLCellValues.Number;

            var tableRange = ws.Range(headerFirstRowIndex, headerFirstColumnIndex,
                treatmentsFirstRowIndex + treatmentData.Count, firstHeaderRow.Count);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.ColumnsUsed().AdjustToContents();
            wb.SaveAs(fname);

            Process.Start(fname);
        }

        private class AnthropometricToExport
        {
            public string Title { get; }
            public Lazy<Func<int, string>> GetValueByIndex { get; }
            public Lazy<Predicate<Anthropometric>> Match { get; }

            public AnthropometricToExport(string title, Func<int, string> getValueByIndex, Predicate<Anthropometric> match)
            {
                Title = title;
                GetValueByIndex = new Lazy<Func<int, string>>(() => (getValueByIndex));
                Match = new Lazy<Predicate<Anthropometric>>(() => (match));
            }
        }

        private List<string> GetRowWithMeasure(List<Anthropometric> sortedAnthros, string title, Func<int, string> getValueByIndex, Predicate<Anthropometric> match)
        {
            const string doubleFormat = "#0.##";
            var resultRow = new List<string>();
            resultRow.AddRange(new[] { title, string.Empty, getValueByIndex(0) });
            for (int i = 1; i < sortedAnthros.Count; i++)
            {
                var iValue = getValueByIndex(i);
                if (string.IsNullOrWhiteSpace(iValue))
                {
                    resultRow.AddRange(new[] { string.Empty, string.Empty });
                    continue;
                }
                var getValue = GetDouble(iValue, double.NaN);
                if (!double.IsNaN(getValue))
                {
                    string diffValue = string.Empty;
                    var diff = GetDiffWithPrevious(i, getValueByIndex);
                    if (!double.IsNaN(diff) && Math.Abs(diff) > 0.01)
                    {
                        diffValue = diff.ToString(doubleFormat);
                    }
                    resultRow.AddRange(new[] {getValue.ToString(doubleFormat), diffValue});
                }
                else
                {
                    resultRow.AddRange(new[] { iValue, "Не посчитать" });
                }
            }
            var weightDiffFromLastToFirst = GetDiffFromLastToFirst(sortedAnthros, getValueByIndex, match);
            resultRow.Add(double.IsNaN(weightDiffFromLastToFirst) || Math.Abs(weightDiffFromLastToFirst) < 0.01 ? string.Empty : weightDiffFromLastToFirst.ToString(doubleFormat));

            return resultRow;
        }

        private double GetDiffFromLastToFirst(List<Anthropometric> sortedAnthros, Func<int, string> getValueByIndex, Predicate<Anthropometric> match)
        {
            var nan = double.NaN;
            var firstMeasureIndex = sortedAnthros.FindIndex(match);
            if (firstMeasureIndex < 0)
            {
                return nan;
            }
            var firstMeasure = getValueByIndex(firstMeasureIndex);
            var lastMeasureIndex = sortedAnthros.FindLastIndex(match);
            if (lastMeasureIndex < 0 || lastMeasureIndex == firstMeasureIndex)
            {
                return nan;
            }
            var lastMeasure = getValueByIndex(lastMeasureIndex);

            double lastMeasureValue = GetDouble(lastMeasure, double.NaN);
            double firstMeasureValue = GetDouble(firstMeasure, double.NaN);

            if (!double.IsNaN(lastMeasureValue) && !double.IsNaN(firstMeasureValue))
            {
                return lastMeasureValue - firstMeasureValue;
            }
            return nan;
        }

        private double GetDiffWithPrevious(int i, Func<int, string> getValueByIndex)
        {
            var previousValue = double.NaN;
            for (var j = i - 1; j >= 0; j--)
            {
                var oreviousStringValue = getValueByIndex(j);
                if (string.IsNullOrWhiteSpace(oreviousStringValue))
                {
                    continue;
                }
                previousValue = GetDouble(oreviousStringValue, double.NaN);
                break;
            }
            if (double.IsNaN(previousValue))
            {
                return double.NaN;
            }
            var value = GetDouble(getValueByIndex(i), double.NaN);
            if (double.IsNaN(value))
            {
                return double.NaN;
            }
            return value - previousValue;
        }

        private static double GetDouble(string value, double defaultValue)
        {
            double result;

            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        private void DoctorVisitsViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if(originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if(row != null)
                {
                    EditDoctorButton_Click(null, null);
                }
            }
        }

        private void NewDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
                return;
            ProcessUserDialog<NewEditDoctorWindow>(() => model.RefreshDoctors(), new ResolverOverride[] { new ParameterOverride("customer", model.CurrentCustomer) });

        }

        private void EditDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedDoctor != null && model.CurrentCustomer != null)
            {
                ProcessUserDialog<NewEditDoctorWindow>(() => model.RefreshDoctors(), new ResolverOverride[] { new ParameterOverride("doctor", model.SelectedDoctor), new ParameterOverride("customer", model.CurrentCustomer) });
            }
        }

        private void DeleteDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedDoctor == null)
                return;
            TonusWindow.Confirm("Удаление",
                 "Удалить посещение за " + model.SelectedDoctor.CreatedOn.ToString("dd.MM.yyyy") + "?",
                    e1 =>
                    {
                        if((e1.DialogResult ?? false))
                        {
                            ClientContext.DeleteObject("DoctorVisits", model.SelectedDoctor.Id);
                            model.RefreshDoctors();
                        }
                    });
        }

        private void NewNutritionButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
                return;
            ProcessUserDialog<NewEditNutritionWindow>(() => model.RefreshNutritions(), new ResolverOverride[] { new ParameterOverride("customer", model.CurrentCustomer) });
        }

        private void EditNutritionButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedNutrition != null && model.CurrentCustomer != null)
            {
                ProcessUserDialog<NewEditNutritionWindow>(() => model.RefreshNutritions(), new ResolverOverride[] { new ParameterOverride("nutrition", model.SelectedNutrition), new ParameterOverride("customer", model.CurrentCustomer) });
            }
        }

        private void DeleteNutritionButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedNutrition == null)
                return;
            TonusWindow.Confirm("Удаление",
                 "Удалить запись за " + model.SelectedNutrition.Date.ToString("dd.MM.yyyy") + "?",
                    e1 =>
                    {
                        if((e1.DialogResult ?? false))
                        {
                            ClientContext.DeleteObject("Nutritions", model.SelectedNutrition.Id);
                            model.RefreshNutritions();
                        }
                    });
        }

        private void NutritionViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if(originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if(row != null)
                {
                    EditNutritionButton_Click(null, null);
                }
            }
        }

        private void MeasuresViewGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
            if(originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if(row != null)
                {
                    EditMeasureButton_Click(null, null);
                }
            }
        }

        private void NewMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.CurrentCustomer == null)
                return;
            ProcessUserDialog<NewEditMeasureWindow>(() => model.RefreshMeasures(), new ResolverOverride[] { new ParameterOverride("customer", model.CurrentCustomer) });
        }

        private void EditMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedMeasure != null && model.CurrentCustomer != null)
            {
                ProcessUserDialog<NewEditMeasureWindow>(() => model.RefreshMeasures(), new ResolverOverride[] { new ParameterOverride("measure", model.SelectedMeasure), new ParameterOverride("customer", model.CurrentCustomer) });
            }
        }

        private void DeleteMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedMeasure == null)
                return;
            TonusWindow.Confirm("Удаление",
                 "Удалить замеры за " + model.SelectedMeasure.Date.ToString("dd.MM.yyyy") + "?",
                    e1 =>
                    {
                        if((e1.DialogResult ?? false))
                        {
                            ClientContext.DeleteObject("CustomerMeasures", model.SelectedMeasure.Id);
                            model.RefreshMeasures();
                        }
                    });
        }

        private void SaveStatusesButton_Click(object sender, RoutedEventArgs e)
        {
            model.SaveStatuses();
        }
    }
}
