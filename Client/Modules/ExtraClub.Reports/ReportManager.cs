using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using ExtraClub.UIControls.Interfaces;
using ExtraClub.UIControls.Windows;

namespace ExtraClub.Reports
{
    internal class ReportManager : IReportManager
    {
        readonly string _root = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

        public void ProcessPdfReport(Func<string> method)
        {
            try
            {
                var fname = Path.Combine(_root, Guid.NewGuid() + ".pdf");

                var srcFn = Guid.NewGuid() + ".html";
                using(var sw = new StreamWriter(srcFn))
                {
                    sw.Write(method());
                }
                var fn2 = "temp_" + Guid.NewGuid() + ".pdf";
                var psi = new ProcessStartInfo("wkhtmltopdf.exe", srcFn + " " + fn2) { CreateNoWindow = true };

                Process.Start(psi)?.WaitForExit();

                File.Delete(srcFn);
                File.Move(fn2, fname);

                Process.Start(fname);
            }
            catch(Exception ex)
            {
                ExtraWindow.Confirm("Ошибка", "При генерации отчета возникла ошибка:\n" + ex.Message + "\n\nПопробовать еще раз?", wnd =>
                {
                    if(wnd.DialogResult ?? false)
                    {
                        ProcessPdfReport(method);
                    }
                });
            }
        }

        public void PrintHtmlToPdf(string html)
        {
            try
            {
                var fname = Path.Combine(_root, Guid.NewGuid() + ".pdf");

                var srcFn = Guid.NewGuid() + ".html";
                using(var sw = new StreamWriter(srcFn))
                {
                    sw.Write("<html><head>  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">    </head>    <body>");

                    sw.Write(html);

                    sw.Write("</body></html>");
                }
                var fn2 = "temp_" + Guid.NewGuid() + ".pdf";
                var psi = new ProcessStartInfo("wkhtmltopdf.exe", srcFn + " " + fn2) { CreateNoWindow = true };

                Process.Start(psi)?.WaitForExit();

                File.Delete(srcFn);
                File.Move(fn2, fname);

                Process.Start(fname);
            }
            catch(Exception ex)
            {
                ExtraWindow.Confirm("Ошибка", "При печати текста возникла ошибка:\n" + ex.Message + "\n\nПопробовать еще раз?",
                    wnd =>
                    {
                        if(wnd.DialogResult ?? false)
                        {
                            PrintHtmlToPdf(html);
                        }
                    });
            }
        }

        public void PrintTextToPdf(List<string> text)
        {
            PrintHtmlToPdf(String.Join("<br>", text));
        }

        public void ExportObjectToExcel<T>(string title, ICollection<T> objectList, params ColumnInfo<T>[] columns)
        {
            var fname = Path.Combine(Path.GetTempPath(),
                $"{title}_{DateTime.Now.ToString(CultureInfo.CurrentCulture).Replace(":", "").Replace("/", "")}.xlsx");

            ExportObjectsToTable(fname, title, objectList, columns);

            Process.Start(fname);
        }

        private void ExportObjectsToTable<T>(string destination, string title, ICollection<T> objectList, IList<ColumnInfo<T>> columns)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Отчет");

            for(var index = 0; index < columns.Count; index++)
            {
                ws.Cell(1, index + 1).SetValue(columns[index].Header);
            }

            var i = String.IsNullOrWhiteSpace(title) ? 1 : 2;
            foreach(var obj in objectList)
            {
                var j = 1;
                foreach(var col in columns)
                {
                    ws.Cell(i, j).SetValue(col.GetMethod(obj));
                    j++;
                }
                i++;
            }

            ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(0x39, 0x6b, 0xa7);
            ws.Row(1).Style.Font.SetFontColor(XLColor.White);

            if(!String.IsNullOrWhiteSpace(title))
            {
                ws.FirstRow().InsertRowsAbove(1);
                ws.Range(1, 1, 1, columns.Count).Merge().SetValue(title);
                ws.Range(2, 1, objectList.Count + 1, columns.Count).SetAutoFilter();
            }
            ws.ColumnsUsed().AdjustToContents();
            wb.SaveAs(destination);
        }

        public void ExportDataTableToExcel(string title, DataTable dataTable)
        {
            if(dataTable?.Columns.Count > 0 && (dataTable?.Columns[0].ColumnName?.StartsWith("_") ?? false))
            {
                dataTable = dataTable.Copy();
                dataTable.Columns.RemoveAt(0);
            }

            var fname = Path.Combine(Path.GetTempPath(),
                $"{title}_{DateTime.Now.ToString(CultureInfo.CurrentCulture).Replace(":", "").Replace("/", "")}.xlsx");
            ConvertToXlsx(fname, title, dataTable);

            Process.Start(fname);
        }

        public static void ConvertToXlsx(string destination, string title, DataTable dataTable)
        {
            var wb = new XLWorkbook();
            dataTable.TableName = String.IsNullOrEmpty(title)
               ? "Отчет"
               : title.Length > 30 ? title.Substring(0, 31) : title;
            wb.Worksheets.Add(dataTable);
            wb.SaveAs(destination);
        }
    }
}
