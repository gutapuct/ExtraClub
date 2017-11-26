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
using System.Xml.Serialization;
using ExtraClub.ServiceModel.Reports;
using System.IO;
using ExtraClub.ServiceModel.Reports.ClauseParameters;
using ExtraClub.ServiceModel.Reports.ClauseParameters.Customers;
using ExtraClub.ServiceModel;
using System.ComponentModel;
using ExtraClub.UIControls.Windows;
using ExtraClub.ServiceModel.Reports.ReportColumns;
using ExtraClub.UIControls;

namespace ExtraClub.Reports.Views.ContainedControls.ReportDesigner
{
    public partial class NewEditReportWindow : INotifyPropertyChanged
    {
        ClauseContainer container;

        public Dictionary<Type, string> BaseTypes { get; set; }

        public List<Triple> Columns { get; set; }
        public List<TripleGuid> Roles { get; set; }

        public ReportInfoInt Report { get; set; }

        private Type _BaseType;
        public Type BaseType
        {
            get
            {
                return _BaseType;
            }
            set
            {
                if (_BaseType == null)
                {
                    _BaseType = value;
                    sp.Content = container = new ClauseContainer(BaseType);
                    container.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    container.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    Columns = ReportColumnsRegistry.GetColumns(BaseType).Select(i => new Triple { Key = i.Key, Value = i.Value, Check = true }).ToList();
                    OnPropertyChanged("Columns");
                    OnPropertyChanged("TargetType");
                    OnPropertyChanged("SelectAllColumns");
                    return;
                }

                ExtraWindow.Confirm("Смена типа отчета", "Вы действительно хотите изменить тип отчета?\nВсе текущие изменения будут утеряны.",
                    wnd =>
                    {
                        if (_BaseType != null && _BaseType != value && (wnd.DialogResult ?? false))
                        {
                            _BaseType = value;
                            sp.Content = container = new ClauseContainer(BaseType);
                            container.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            container.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            Columns = ReportColumnsRegistry.GetColumns(BaseType).Select(i => new Triple { Key = i.Key, Value = i.Value, Check = true }).ToList();
                            OnPropertyChanged("Columns");
                            OnPropertyChanged("TargetType");
                            OnPropertyChanged("SelectAllColumns");
                        }
                    });
            }
        }

        public bool SelectAllColumns
        {
            get
            {
                return !Columns.Any(i => !i.Check);
            }
            set
            {
                Columns.ForEach(i =>
                {
                    i.Check = value;
                });
                OnPropertyChanged("SelectAllColumns");
            }
        }

        public NewEditReportWindow(ClientContext context, ReportInfoInt report)
            : base(context)
        {
            BaseTypes = new Dictionary<Type, string>();
            BaseTypes.Add(typeof(Customer), "Клиент");
            BaseTypes.Add(typeof(CustomerTarget), "Цель");
            BaseTypes.Add(typeof(TreatmentEvent), "Основная услуга");
            BaseTypes.Add(typeof(Treatment), "Оборудование");
            BaseTypes.Add(typeof(Ticket), "Абонемент");
            BaseTypes.Add(typeof(CustomerCard), "Карта");
            BaseTypes.Add(typeof(GoodSale), "Продажа бара");
            BaseTypes.Add(typeof(Spending), "Затрата");
            BaseTypes.Add(typeof(Good), "Товар");
            BaseTypes.Add(typeof(Employee), "Сотрудник");

            InitializeComponent();
            Roles = new List<TripleGuid>();
            if (String.IsNullOrEmpty(report.Name))
            {
                Report = new ReportInfoInt
                {
                    Key = Guid.Empty.ToString(),
                    Type = ReportType.Configured,
                };
                BaseType = typeof(Customer);

                Roles.AddRange(context.GetRoles().Select(i => new TripleGuid { Key = i.RoleId, Value = i.RoleName, Check = false }));
            }
            else
            {

                var knownTypes = new List<Type> { typeof(Clause), typeof(OrClause),
                typeof(AndClause),
                typeof(FiniteClause),
                typeof(Clause)};
                knownTypes.AddRange(ClauseRegistry.GetRelatedAttributes(null));

                var ser = new XmlSerializer(typeof(Clause), knownTypes.ToArray());

                var ms = new MemoryStream(report.XmlClause);
#if BEAUTINIKA
                _BaseType = Type.GetType(report.BaseTypeName + ", Beautinika.ServiceModel");
#else
                _BaseType = Type.GetType(report.BaseTypeName + ", ExtraClub.ServiceModel");
#endif
                Columns = ReportColumnsRegistry.GetColumns(BaseType).Select(i => new Triple { Key = i.Key, Value = i.Value, Check = report.CustomFields.Contains(i.Key) }).ToList();
                sp.Content = container = new ClauseContainer(BaseType, (Clause)ser.Deserialize(ms));
                container.CurrentChain.SetContext(context);
                container.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                container.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Report = report;
                Roles.AddRange(context.GetRoles().Select(i => new TripleGuid { Key = i.RoleId, Value = i.RoleName, Check = Report.RoleIds.Contains(i.RoleId) }));
            }
            DataContext = this;
            OnPropertyChanged("SelectAllColumns");
        }

        private byte[] SerializeChain()
        {
            var knownTypes = new List<Type> { typeof(Clause), typeof(OrClause),
                typeof(AndClause),
                typeof(FiniteClause),
                typeof(Clause)};
            knownTypes.AddRange(ClauseRegistry.GetRelatedAttributes(null));

            var ser = new XmlSerializer(typeof(Clause), knownTypes.ToArray());
            var ms = new MemoryStream();
            ser.Serialize(ms, container.CurrentChain);
            ms.Position = 0;
            return ms.ToArray();
        }

        private List<string> GetCheckedFields()
        {
            if (Columns == null) return new List<string>();
            return Columns.Where(i => i.Check).Select(i => i.Key).ToList();
        }

        private void AssetButton_Click(object sender, RoutedEventArgs e)
        {
            Report.XmlClause = SerializeChain();
            Report.CustomFields = GetCheckedFields();
            Report.BaseTypeName = BaseType.FullName;
            Report.RoleIds = Roles.Where(i => i.Check).Select(i => i.Key).ToArray();
            _context.PostUserReport(Report);

            DialogResult = true;
            Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class Triple : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public string Value { get; set; }
        bool _check;
        public bool Check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
                OnPropertyChanged("Check");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class TripleGuid
    {
        public Guid Key { get; set; }
        public string Value { get; set; }
        public bool Check { get; set; }
    }
}
