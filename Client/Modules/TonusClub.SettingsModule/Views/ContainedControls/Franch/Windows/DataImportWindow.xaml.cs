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
using System.Windows.Shapes;
using TonusClub.ServiceModel;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.IO;
using TonusClub.UIControls;

namespace TonusClub.SettingsModule.Views.ContainedControls.Franch.Windows
{
    public partial class DataImportWindow
    {
        string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                UpdateDataTable(value);
                OnPropertyChanged("FileName");
            }
        }
        bool firstRowColumnNames = true;
        public bool FirstRowColumnNames
        {
            get
            {
                return firstRowColumnNames;
            }
            set
            {
                firstRowColumnNames = value;
                UpdateDataTable(_fileName);
                OnPropertyChanged("FirstRowColumnNames");
            }
        }

        public int ColumnsNumber { get; set; }

        public System.Data.DataTable CustomersTable { get; set; }

        public int LastnameCol { get; set; }
        public int FirstnameCol { get; set; }
        public int MiddlenameCol { get; set; }
        public int BirthdayCol { get; set; }
        public int PaspNumCol { get; set; }
        public int PaspEmitCol { get; set; }
        public int PastEmitPlaceCol { get; set; }
        public int PhoneCol { get; set; }

        private void UpdateDataTable(string value)
        {
            try
            {
                if (!new FileInfo(value).Exists) return;
                CustomersTable = new System.Data.DataTable();
                var app = new Microsoft.Office.Interop.Excel.Application();
                var wb = app.Workbooks.Open(_fileName, Type.Missing, true);
                var ws = wb.Worksheets[1] as Worksheet;

                var lastCol = ws.get_Range("a1").get_End(XlDirection.xlToRight).Column;
                var lastRow = ws.get_Range(ConvertToLetter(lastCol) + "65536").get_End(XlDirection.xlUp).Row;
                var r = ws.get_Range("a1", ConvertToLetter(lastCol) + lastRow.ToString());
                for (var i = 0; i < lastCol; i++)
                {
                    var cn = "Столбец " + (i + 1);
                    if (FirstRowColumnNames)
                    {
                        cn = r[1, i + 1].Value.ToString();
                    }
                    CustomersTable.Columns.Add(cn, typeof(string));
                }
                for (var j = (FirstRowColumnNames) ? 2 : 1; j <= lastRow; j++)
                {
                    var row = CustomersTable.Rows.Add();
                    for (var i = 0; i < lastCol; i++)
                    {
                        try
                        {
                            if (r[j, i + 1].Value != null)
                            {
                                row[i] = r[j, i + 1].Value.ToString();
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                ColumnsNumber = lastCol;
                OnPropertyChanged("ColumnsNumber");
                OnPropertyChanged("CustomersTable");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.GetType().FullName + "\n\n" + ex.StackTrace, "Ошибка при импорте");
            }
        }


        public DataImportWindow(ClientContext context)
            : base(context)
        {
            LastnameCol = 1;
            FirstnameCol = 2;
            MiddlenameCol = 3;
            BirthdayCol = 4;
            PaspNumCol = 5;
            PaspEmitCol = 6;
            PastEmitPlaceCol = 7;
            PhoneCol = 8;

            InitializeComponent();
            DataContext = this;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "Таблицы Excel|*.xls;*.xlsx"
            };


            if (dlg.ShowDialog() ?? false)
            {
                FileName = dlg.FileName;
            }

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

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            var dlg = new ImportPreviewWindow(_context, ImportCustomers());
            dlg.ClosedParams = w =>
            {
                if (w.DialogResult ?? false)
                {
                    Close();
                }
            };
            dlg.ShowDialog();
        }

        private bool Validate()
        {
            if (CustomersTable == null || CustomersTable.Rows.Count == 0) return false;
            if (LastnameCol > ColumnsNumber) return false;
            if (FirstnameCol > ColumnsNumber) return false;
            if (MiddlenameCol > ColumnsNumber) return false;
            if (PaspNumCol > ColumnsNumber) return false;
            if (PastEmitPlaceCol > ColumnsNumber) return false;
            if (PhoneCol > ColumnsNumber) return false;
            if (BirthdayCol > ColumnsNumber) return false;
            if (PaspEmitCol > ColumnsNumber) return false;

            return true;
        }

        private List<Customer> ImportCustomers()
        {
            var res = new List<Customer>();
            foreach (DataRow row in CustomersTable.Rows)
            {
                if (String.IsNullOrEmpty(row[LastnameCol - 1] as string) || String.IsNullOrEmpty(row[FirstnameCol - 1] as string))
                {
                    continue;
                }
                var c = new Customer
                {
                    ClubId = _context.CurrentDivision.Id,
                    Gender = false,
                    SmsList = true,
                    LastName = row[LastnameCol - 1].ToString(),
                    FirstName = row[FirstnameCol - 1].ToString(),
                    MiddleName = row[MiddlenameCol - 1].ToString(),
                    PasspNumber = row[PaspNumCol - 1].ToString(),
                    PasspEmitPlace = row[PastEmitPlaceCol - 1].ToString(),
                    Phone2 = row[PhoneCol - 1].ToString()
                };
                DateTime bd;
                if (DateTime.TryParse(row[BirthdayCol - 1].ToString(), out bd))
                {
                    c.Birthday = bd;
                }
                if (DateTime.TryParse(row[PaspEmitCol - 1].ToString(), out bd))
                {
                    c.PasspEmitDate = bd;
                }
                res.Add(c);
            }
            return res;
        }

    }
}
