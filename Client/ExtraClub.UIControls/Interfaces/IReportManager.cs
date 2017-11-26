using System;
using System.Collections.Generic;
using System.Data;

namespace ExtraClub.UIControls.Interfaces
{
    public interface IReportManager
    {
        void ExportDataTableToExcel(string title, DataTable dataTable);
        void ExportObjectToExcel<T>(string title, ICollection<T> objectList, params ColumnInfo<T>[] columns);
        void PrintTextToPdf(List<string> text);
        void ProcessPdfReport(Func<string> method);
        void PrintHtmlToPdf(string html);
    }
}