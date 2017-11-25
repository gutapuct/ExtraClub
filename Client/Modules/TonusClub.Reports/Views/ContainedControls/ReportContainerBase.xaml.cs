using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ClosedXML.Excel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using TonusClub.Infrastructure;
using TonusClub.Reports.Business;
using TonusClub.ServiceModel;
using TonusClub.ServiceModel.Reports;
using TonusClub.UIControls;
using TonusClub.UIControls.Windows;
using Brushes = System.Windows.Media.Brushes;
using File = System.IO.File;
using System.Net;

namespace TonusClub.Reports.Views.ContainedControls
{
    public partial class ReportContainerBase
    {
        private Func<IEnumerable<ReportParamInt>, object> _generationFunc;

        private static readonly Dictionary<int, Color> HeaderColorsDictionary;
        static ReportContainerBase()
        {
            HeaderColorsDictionary = new Dictionary<int, Color>
            {
                {0, Color.FromArgb(0x39, 0x6b, 0xa7)},
                {1, Color.FromArgb(0x74, 0xa6, 0xe2)},
                {2, Color.FromArgb(0x83, 0xb5, 0xf1)},
                {3, Color.FromArgb(0x9a, 0xcc, 0xff)}
            };
        }

        public List<ReportColumnInfo> ReportColumns { get; set; }


        private ReportInfoInt _reportInfo;
        public ReportInfoInt ReportInfoInt
        {
            get
            {
                return _reportInfo;
            }
            set
            {
                _reportInfo = value;
                OnPropertyChanged("ReportInfoInt");
                if (ReportInfoInt.Type == ReportType.CodeParams || ReportInfoInt.Type == ReportType.ConfiguredParams)
                {
                    Dispatcher.BeginInvoke(new Action(() => GenerateClick(null, null)));
                }
            }
        }

        public bool SelectAllColumns
        {
            get
            {
                if (ReportColumns == null)
                {
                    return true;
                }
                return !ReportColumns.Any(i => !i.Check);
            }
            set
            {
                if (ReportColumns != null)
                {
                    ReportColumns.ForEach(i => i.Check = value);
                    OnPropertyChanged("SelectAllColumns");
                }
            }
        }

        private DataTable ResultTable => ResultGrid?.ItemsSource as DataTable;

        private bool _isReportProgress;
        public bool IsReportProgress
        {
            get
            {
                return _isReportProgress;
            }
            set
            {
                _isReportProgress = value;
                OnPropertyChanged("IsReportProgress");
            }
        }

        public Visibility TaskButtonVisibility
        {
            get
            {
                if (ResultTable == null)
                {
                    return Visibility.Collapsed;
                }

                try
                {
                    if (ResultTable.Columns.Contains("_customerId") ||
                        Type.GetType(ResultTable.ExtendedProperties["EntityType"] + ", TonusClub.ServiceModel") == typeof(Customer))
                    {
                        return Visibility.Visible;
                    }
                }
                catch
                {
                    // ignored
                }
                return Visibility.Collapsed;
            }
        }

        readonly decimal _red;
        readonly decimal _orange;
        readonly decimal _yellow;

        public ReportContainerBase(decimal yellow, decimal orange, decimal red)
        {
            DataContext = this;

            InitializeComponent();
            IsReportProgress = false;

            Style s = new Style(typeof(GridViewGroupRow));


            Setter newSetter = new Setter(GridViewGroupRow.ShowHeaderAggregatesProperty, false);

            s.Setters.Add(newSetter);
            s.Seal();

            ResultGrid.GroupRowStyle = s;

            _red = red;
            _orange = orange;
            _yellow = yellow;
        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultTable == null)
            {
                return;
            }

            if (ResultTable.ExtendedProperties.ContainsKey("DotsFormat"))
            {
                DotsFormatExporter.Export(ResultTable);
            }
            else
            {
                ExportReportToExcel();
            }
        }

        private void ExportReportToExcel()
        {
            var fname = Path.Combine(Path.GetTempPath(),
                $"{_reportInfo.Name}_{DateTime.Now.ToString(CultureInfo.CurrentCulture).Replace(":", "").Replace("/", "")}.xlsx");

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Отчет");

            var columns = ResultGrid.Columns.Cast<GridViewColumn>().Where(i => i.IsVisible).ToArray();
            for (var i = 0; i < columns.Length; i++)
            {
                ws.Cell(1, i+1).SetValue(columns[i].Header);
            }

            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromColor(HeaderColorsDictionary[1]);

            var rowNumber = 2;

            var exportRow = new Action<DataRow>(row =>
            {
                var colNumber = 1;
                foreach (var col in columns)
                {
                    ws.Cell(rowNumber, colNumber++).SetValue(row[col.UniqueName]);
                }
                rowNumber++;
            });

            var exportRows = new Action<IEnumerable<DataRow>>(rows =>
            {
                foreach (var row in rows)
                {
                    exportRow(row);
                }
            });

            if (ResultGrid.Items.Groups == null)
            {
                exportRows(ResultGrid.Items.Cast<DataRow>());
            }
            else
            {
                foreach (dynamic group in ResultGrid.Items.Groups)
                {
                    string name = group.Name.ToString();
                    ws.Range(rowNumber, 1, rowNumber, columns.Length)
                        .Merge()
                        .SetValue(name)
                        .Style.Fill.BackgroundColor = XLColor.FromColor(HeaderColorsDictionary[2]);
                    rowNumber++;
                    foreach (var row in group.Items)
                    {
                        exportRow(row);
                    }
                }
            }

            ws.ColumnsUsed().AdjustToContents();
            ws.SetAutoFilter();

            wb.SaveAs(fname);

            Process.Start(fname);
        }

        private void PdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var root = Path.GetTempPath();
                var str =
                    $"<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>{ResultGrid.ToHtml(true, true)}</body></html>";
                var fname = Path.Combine(root, (_reportInfo.Name + DateTime.Now)
                    .Replace("*", "_")
                    .Replace("@", "_")
                    .Replace("$", "_")
                    .Replace("%", "_")
                    .Replace("^", "_")
                    .Replace("&", "_")
                    .Replace(".", "_")
                    .Replace(":", "_")
                    .Replace("\\", "_")
                    .Replace("/", "_")
                    .Replace("?", "_") + ".pdf");
                var srcFn = Guid.NewGuid() + ".html";
                using (var sw = new StreamWriter(srcFn))
                {
                    sw.Write(str);
                }

                var fn2 = "temp_" + Guid.NewGuid() + ".pdf";
                var psi = new ProcessStartInfo("wkhtmltopdf.exe", srcFn + " " + fn2) { CreateNoWindow = true };
                var p = Process.Start(psi);
                p?.WaitForExit();
                File.Delete(srcFn);
                File.Move(fn2, fname);

                Process.Start(fname);
            }
            catch (Exception ex)
            {
                TonusWindow.Alert("Ошибка", "При экспорте отчета возникла ошибка:\n" + ex.Message);
            }
        }

        public void GenerateClick(object sender, RoutedEventArgs e)
        {
            IsReportProgress = true;
            ThreadPool.QueueUserWorkItem(_ => GenerateInternal());
        }

        protected virtual void GenerateInternal()
        {
            IsReportProgress = true;
            try
            {
                var res = _generationFunc(ReportInfoInt.Parameters);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //ResultGrid.AutoGenerateColumns = false;
                    ResultGrid.ItemsSource = null;
                    ResultGrid.Columns.Clear();
                    //ResultGrid.AutoGenerateColumns = true;
                    ResultGrid.ItemsSource = res;
                    OnPropertyChanged("TaskButtonVisibility");
                    var dt = res as DataTable;
                    if (dt != null)
                    {
                        var detailed = dt.ExtendedProperties.ContainsKey("Detailed");
                        var vert2 = dt.ExtendedProperties.ContainsKey("VerticalColor2");
                        var isFin = dt.Columns.Count > 0 && dt.Columns[0].ColumnName == "_color";
                        var flag = false;
                        int index = 0;
                        var vind = 3;//0+ 1+ 2- 3-
                        var old = new bool[0];
                        if (ReportColumns != null)
                        {
                            old = ReportColumns.Select(i => i.Check).ToArray();
                        }
                        ReportColumns = ResultGrid.Columns.Cast<GridViewColumn>().Select(i => new ReportColumnInfo { Name = i.Header.ToString(), Column = i }).ToList();
                        if (ResultGrid.Columns.Count > 0 && ResultGrid.Columns[0].Header.ToString().StartsWith("_"))
                        {
                            ReportColumns.RemoveAt(0);
                        }
                        if (ReportColumns.Count == old.Length)
                        {
                            for (var i = 0; i < old.Length; i++)
                            {
                                ReportColumns[i].Check = old[i];
                            }
                        }
                        OnPropertyChanged("ReportColumns");
                        foreach (var column in ResultGrid.Columns)
                        {
                            column.MinWidth = 100;
                            if ((vind == 0 || vind == 1) && vert2)
                            {
                                column.Background = Brushes.AliceBlue;
                            }
                            if (++vind == 4) vind = 0;
                            var type = dt.Columns[column.UniqueName].DataType;
                            if (ReportInfoInt.Key != "GetAllCustomersEx")
                            {
                                if (type == typeof(decimal) || type == typeof(double) || type == typeof(int))
                                {
                                    column.AggregateFunctions.Add(new SumFunction { Caption = "Сумма: ", ResultFormatString = "{0:n2}" });
                                    column.AggregateFunctions.Add(new AverageFunction { Caption = "Среднее: ", ResultFormatString = " {0:n2}" });
                                }
                                else
                                {
                                    if (!flag)
                                    {
                                        column.AggregateFunctions.Add(new TotalCaptionFunction());
                                        flag = true;
                                    }
                                }
                            }
                            if (type == typeof(decimal) || type == typeof(double))
                                ((GridViewDataColumn)column).DataFormatString = "{0:n2}";
                            if (isFin && dt.Columns[index].ExtendedProperties.ContainsKey("MonthColumn"))
                            {
                                column.CellStyleSelector = new BackgroundStyleSelectorFinance(index);
                            }
                            else
                            {
                                if (detailed) column.CellStyleSelector = new BackgroundStyleSelector(index, _red, _orange, _yellow);
                            }
                            index++;
                            if (column.UniqueName.Length > 0 && column.UniqueName[0] == '_')
                            {
                                column.IsVisible = false;
                            }
                        }
                        ResultGrid.UpdateLayout();
                    }
                }));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    TonusWindow.Alert("Ошибка", "При генерации отчета возникла ошибка:\n" + ex.Message);
                }));
            }
            finally
            {
                IsReportProgress = false;
                //using (var wc = new WebClient())
                //{
                //    wc.DownloadStringAsync(new Uri(
                //        "http://asu.flagmax.ru/publicapi/externallog?"
                //        + $"companyid={ClientContext.CurrentCompany.CompanyId}"
                //        + $"&userid={ClientContext.CurrentUser.UserId}"
                //        + $"&message={_reportInfo.Name}")
                //    );
                //}
            }
        }

        private void ResultGrid_Grouped(object sender, GridViewGroupedEventArgs e)
        {
            (((ColumnGroupDescriptor)(e.GroupDescriptor))).SortDirection = null;
            var flag = false;
            foreach (var column in ResultGrid.Columns)
            {
                if (column.IsVisible && !column.AggregateFunctions.Any() && !flag)
                {
                    column.AggregateFunctions.Add(new TotalCaptionFunction());
                    flag = true;
                    continue;
                }
                if (column.AggregateFunctions.Any(i => i is TotalCaptionFunction))
                {
                    column.AggregateFunctions.RemoveAt(0);
                }
            }
        }

        public static double GetFontSize(int count)
        {
            return (4 - count) + 12;
        }

        private void ResultGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsRowClicked(e))
            {
                var row = (DataRow)ResultGrid.SelectedItem;
                var entityType = typeof(object);

                if (row.Table.ExtendedProperties.ContainsKey("EntityType"))
                {
                    entityType = Type.GetType(row.Table.ExtendedProperties["EntityType"] + ", TonusClub.ServiceModel");
                }
                var isCustomer = false;
                if (ResultTable?.Columns.Count > 0)
                {
                    isCustomer = ResultTable.Columns[0].ColumnName == "_customerId";
                }
                if (entityType == typeof(Customer) || isCustomer)
                {
                    NavigationManager.MakeClientRequest((Guid)row[0]);
                }
                if (entityType == typeof(CustomerTarget))
                {
                    NavigationManager.MakeCustomerTargetRequest((Guid)row[0]);
                }
                if (entityType == typeof(TreatmentEvent))
                {
                    NavigationManager.MakeTreatmentEventRequest((Guid)row[0]);
                }
                if (entityType == typeof(Ticket))
                {
                    NavigationManager.MakeTicketRequest((Guid)row[0]);
                }
                if (entityType == typeof(CustomerCard))
                {
                    NavigationManager.MakeCustomerCardRequest((Guid)row[0]);
                }
                if (entityType == typeof(GoodSale))
                {
                    NavigationManager.MakeGoodSalesRequest((Guid)row[0]);
                }
                if (entityType == typeof(Spending))
                {
                    NavigationManager.MakeSpendingRequest((Guid)row[0]);
                }
                if (entityType == typeof(Employee))
                {
                    NavigationManager.MakeEmployeeRequest((Guid)row[0]);
                }
                if (entityType == typeof(Good))
                {
                    this.ParentOfType<ReportsLargeView>().ProcessGoodDetailsReport((Guid)ReportInfoInt.Parameters[2].InstanceValue, (Guid)row[0]);
                }
            }
        }

        private void SaveParamsClick(object sender, RoutedEventArgs e)
        {
            if (ReportInfoInt.Parameters.Count == 0)
            {
                TonusWindow.Alert("Ошибка", "Невозможно сохранить параметры, так как отчет их не содержит.");
                return;
            }
            TonusWindow.Prompt("Сохранение параметров",
                "Укажите название набора сохраненных параметров:",
                ReportInfoInt.Name + " - новый набор параметров",
                EditClosed);
        }

        private void EditClosed(PromptWindow wnd)
        {
            if (wnd.DialogResult ?? false)
            {
                if (String.IsNullOrWhiteSpace(wnd.TextResult))
                {
                    wnd.TextResult = ReportInfoInt.Name + " - новый набор параметров";
                }
                var pars = ReportInfoInt.Parameters;
                ClientContext.PostParameters(ReportInfoInt.Key, pars.ToDictionary(i => i.InternalName, i => i.InstanceValue), wnd.TextResult);
            }
        }

        private void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            var rows = ResultGrid.SelectedItems.Cast<DataRow>().ToArray();

            if (!rows.Any()) return;
            var entityType = typeof(object);
            if (rows.First().Table.ExtendedProperties.ContainsKey("EntityType"))
            {
                entityType = Type.GetType(rows.First().Table.ExtendedProperties["EntityType"] + ", TonusClub.ServiceModel");
            }

            var isCustomer = false;
            if (ResultTable?.Columns.Count > 0)
            {
                isCustomer = ResultTable?.Columns[0].ColumnName == "_customerId";
            }
            if (entityType == typeof(Customer) || isCustomer)
            {
                NavigationManager.MakeCallTaskRequest(rows.Select(i => (Guid)i[0]).ToArray());
            }


        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            TonusWindow.Confirm("Отправка отчета", "Отправить отчет франчайзору?\nПотребуется подключение к интернету.", wnd =>
            {
                if (wnd.DialogResult ?? false)
                {
                    if (ResultTable == null) return;
                    MemoryStream ms = new MemoryStream();
                    ResultGrid.Export(ms, new GridViewExportOptions { Format = ExportFormat.Html, ShowColumnFooters = true, ShowColumnHeaders = true, ShowGroupFooters = true });
                    try
                    {
                        ChannelFactory<ISyncService> cf = new ChannelFactory<ISyncService>("SyncServiceEndpoint");
                        var client = cf.CreateChannel();
                        client.PostFrReport(ClientContext.CurrentCompany.ReportEmail, GetSubject(), GetReportDetailsText(), ms.ToArray());
                        TonusWindow.Alert("Отчет отправлен", "Отчет отправлен успешно");
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        TonusWindow.Alert("Отправить отчет не удалось.", "Пожалуйста, проверьте подключение к интернету.\n" + ex.GetType().ToString() + "\n" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        TonusWindow.Alert("К сожалению, отправить отчет не удалось.", "Ошибка при отправке отчета\n" + ex.GetType().ToString() + "\n" + ex.Message);
                    }
                }
            });
        }

        private string GetSubject()
        {
            return $"Отчет {_reportInfo.Name} от {ClientContext.CurrentUser.FullName}";
        }

        private string GetReportDetailsText()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Франчайзи: {0}\nКлуб: {1}\nНомер ДКК: {6}\nРег. сервер: {2}\nПользователь: {3}\nВремя отправки: {4}\nНазвание отчета: {5}\n\nПараметры:\n",
                ClientContext.CurrentCompany.CompanyName,
                ClientContext.CurrentDivision.Name,
                AppSettingsManager.GetSetting("ServerAddress"),
                ClientContext.CurrentUser.FullName,
                DateTime.Now,
                _reportInfo.Name,
                ClientContext.CurrentDivision.ConcessionNumber);
            foreach (var i in _reportInfo.Parameters)
            {
                sb.AppendFormat("Название: {0}, значение: {1}\n", i.DisplayName, i.InstanceValue);
            }
            return sb.ToString();
        }

    }

    public class ReportColumnInfo : INotifyPropertyChanged
    {
        public string Name { get; set; }
        bool _check = true;
        public bool Check
        {
            get
            {
                return _check;
            }
            set
            {
                if (_check != value)
                {
                    _check = value;
                    Column.IsVisible = value;
                    OnPropertyChanged("Check");
                }
            }
        }
        public GridViewColumn Column { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
