using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Runtime.InteropServices;
using TonusClub.Reports.Views.ContainedControls;
using System.Drawing;
using System.Windows;
using TonusClub.Infrastructure.Extensions;

namespace TonusClub.Reports.Business
{
    class DotsFormatExporter
    {
        private static Dictionary<int, Color> HeaderColorsDictionary;
        static DotsFormatExporter()
        {
            HeaderColorsDictionary = new Dictionary<int, Color>();
            HeaderColorsDictionary.Add(0, Color.FromArgb(0x39, 0x6b, 0xa7));
            HeaderColorsDictionary.Add(1, Color.FromArgb(0x74, 0xa6, 0xe2));
            HeaderColorsDictionary.Add(2, Color.FromArgb(0x83, 0xb5, 0xf1));
            HeaderColorsDictionary.Add(3, Color.FromArgb(0x9a, 0xcc, 0xff));
        }

        internal static void Export(System.Data.DataTable dataTable)
        {
            var diff = Int16.Parse(dataTable.ExtendedProperties["DotsFormat"].ToString());
            var fin = dataTable.ExtendedProperties.ContainsKey("Fin");
            var format = 0;
            if (dataTable.ExtendedProperties.ContainsKey("DefaultCellFormat"))
                format = Int16.Parse(dataTable.ExtendedProperties["DefaultCellFormat"].ToString());
            var app = new Microsoft.Office.Interop.Excel.Application();
            var wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            var ws = wb.Worksheets[1] as Worksheet;

            var vert = 9999;
            if (dataTable.ExtendedProperties.ContainsKey("VerticalHeadersStart"))
                vert = Int16.Parse(dataTable.ExtendedProperties["VerticalHeadersStart"].ToString());

            var i = 1;
            foreach (DataColumn c in dataTable.Columns)
            {
                if (c.ColumnName == "_style") continue;
                var r = ws.get_Range(ConvertToLetter(i) + "1");
                r.Value2 = c.ColumnName;
                if (i++ > vert)
                {
                    r.Orientation = 90;
                }
            }
            var groups = new int[32];
            var rgroups = new int[32];
            var currentGroup = 0;
            var regions = new Dictionary<DataRow, KeyValuePair<string, Range>>();
            int n = 2;
            int rn = 0;
            foreach (DataRow r in dataTable.Rows)
            {
                var gr = r[fin ? 1 : 0].ToString().Count(j => j == '.');
                if (gr > currentGroup)
                {
                    groups[currentGroup] = n;
                    rgroups[currentGroup] = rn-1;
                    currentGroup++;
                }

                while (gr < currentGroup)
                {
                    currentGroup--;
                    var range = ws.get_Range("A" + groups[currentGroup], "A" + (n - 1));

                    regions.Add(dataTable.Rows[rgroups[currentGroup]],
                        new KeyValuePair<string, Range>("A" + groups[currentGroup]+"-"+ "A" + (n - 1), range));

                    range.Rows.Group();

                    if (gr < currentGroup)
                    {
                        n++;
                    }
                }

                i = 0;
                foreach (DataColumn c in dataTable.Columns)
                {
                    if (c.ColumnName == "_style") continue;
                    var range = ws.get_Range(ConvertToLetter(++i) + n);
                    if (i > 2)
                    {
                        if (r[fin ? 2 : 1].ToString().Contains('%'))
                        {
                            range.NumberFormatLocal = "0,00\\%";
                        }
                        else
                        {
                            if (format == 0 && !fin)
                            {
                                range.NumberFormatLocal = "# ##0,00\\ р.";
                            }
                            else
                            {
                                range.NumberFormatLocal = "# ##0";
                            }
                        }
                    }
                    range.Value2 = r[c];
                }
                if (!fin)
                {
                    if (gr > 0 && gr < 5 - diff)
                    {
                        var ra = ws.get_Range("A" + n, ConvertToLetter(dataTable.Columns.Count) + n);
                        ra.Rows.Interior.Color = System.Drawing.ColorTranslator.ToOle(HeaderColorsDictionary[gr - 1 + diff]);

                        if (gr < 4 - diff)
                        {
                            ra.Rows.Font.Size = ReportContainerBase.GetFontSize(gr);
                        }
                        if (gr < 3 - diff)
                        {
                            ra.Rows.Font.Bold = true;
                        }
                    }
                }
                else
                {
                    var ra = ws.get_Range("A" + n, ConvertToLetter(dataTable.Columns.Count) + n);
                    ApplyFinStyle(r[0].ToString(), ra);
                }


                n++;
                rn++;
            }


            while (currentGroup > 1)
            {
                var group = ws.get_Range("A" + groups[--currentGroup], "A" + (n - 1)).Rows.Group();
                n++;
            }
            ws.get_Range("A1", ConvertToLetter(dataTable.Columns.Count) + n).Columns.AutoFit();

            foreach (var r in regions)
            {
                if (r.Key[0].ToString().Length > 3 && r.Key[0].ToString()[3] == '-')
                {
                        r.Value.Value.EntireRow.Hidden = true;
                }
            }

            ws.Outline.SummaryRow = XlSummaryRow.xlSummaryAbove;
            //вертикальная группировка
            if (fin)
            {
                ws.Outline.SummaryColumn = XlSummaryColumn.xlSummaryOnLeft;
                for (i = 4; i < dataTable.Columns.Count-1; i += 4)
                {
                    var range = ws.get_Range(ConvertToLetter(i) + "1", ConvertToLetter(i + 2) + "1");
                    range.Columns.Group();
                    range.EntireColumn.Hidden = true;
                }
            }
            app.Visible = true;

            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(app);
            GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.GetTotalMemory(true);
        }

        private static void ApplyFinStyle(string style, Range ra)
        {
            ra.Font.Name = "Arial";
            ra.Font.Size = 8;
            ra.Rows.RowHeight = 11.25;
            int background = 0;
            int foreground = 0;
            var isBold = false;
            var isItalic = false;
            style = style.Substring(0, 3);
            if (style.In("cs1", "cs2", "cs5"))
            {
                isBold = true;
            }
            if (style.In("cs5", "cs6"))
            {
                isItalic = true;
            }
            switch (style)
            {
                case "cs1":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(204, 192, 218));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(247, 42, 0));
                    break;
                case "cs2":
                case "cs6":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(216, 228, 188));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(0, 0, 0));
                    break;
                case "cs3":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(242, 220, 219));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(0, 0, 0));
                    break;
                case "cs5":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(216, 228, 188));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(79, 129, 189));
                    break;
                case "cs7":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(141,180,226));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(255, 0, 0));
                    break;
                case "cs8":
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(230, 184, 183));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(0, 0, 0));
                    break;
                default:
                    background = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(255, 255, 255));
                    foreground = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(0, 0, 0));
                    break;
            }
            ra.Rows.Interior.Color = background;
            ra.Font.Color = foreground;
            ra.Font.Italic = isItalic;
            ra.Font.Bold = isBold;

            Borders borders = ra.Borders;
            borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;
            borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;
            borders = null;
        }

        public static string ConvertToLetter(int iCol)
        {
            int iAlpha;
            int iRemainder;
            string res = "";
            iAlpha = (int)(iCol / 27);
            iRemainder = iCol - (iAlpha * 26);
            if (iAlpha > 0)
            {
                res = res + (char)(iAlpha + 64);
            }
            if (iRemainder > 0)
            {
                res = res + (char)(iRemainder + 64);
            }
            return res;
        }
    }
}
