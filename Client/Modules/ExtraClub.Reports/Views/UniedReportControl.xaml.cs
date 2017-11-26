using System;
using System.Linq;
using System.Windows.Threading;
using Telerik.Windows.Controls.Charting;
using ExtraClub.Reports.ViewModels;

namespace ExtraClub.Reports.Views
{
    public partial class UniedReportControl
    {
        public UniedReportViewModel Model { get; set; }

        public UniedReportControl(UniedReportViewModel model)
        {
            DataContext = Model = model;
            InitializeComponent();

            radChart.DefaultView.ChartLegend.Header = "";
            radChart.DefaultView.ChartArea.AxisX.LayoutMode = AxisLayoutMode.Between;
            radChart.DefaultView.ChartArea.AxisY.DefaultLabelFormat = "#VAL{N0}";

            radChart1.DefaultView.ChartLegend.Header = "";
            radChart1.DefaultView.ChartArea.AxisX.LayoutMode = AxisLayoutMode.Between;
            radChart1.DefaultView.ChartArea.AxisY.DefaultLabelFormat = "#VAL{N0}";

            amountTickets.DefaultView.ChartLegend.Header = "";
            amountTickets.DefaultView.ChartArea.AxisX.LayoutMode = AxisLayoutMode.Between;
            amountTickets.DefaultView.ChartArea.AxisY.DefaultLabelFormat = "#VAL{N0}";

            var seriesMapping1 = new SeriesMapping { LegendLabel = "Нал" };
            seriesMapping1.SeriesDefinition = new StackedBarSeriesDefinition();
            seriesMapping1.ItemMappings.Add(new ItemMapping("Nal", DataPointMember.YValue));
            seriesMapping1.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory) { });
            radChart1.SeriesMappings.Add(seriesMapping1);

            seriesMapping1 = new SeriesMapping { LegendLabel = "Безнал" };
            seriesMapping1.SeriesDefinition = new StackedBarSeriesDefinition();
            seriesMapping1.ItemMappings.Add(new ItemMapping("Beznal", DataPointMember.YValue));
            seriesMapping1.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory) { });
            radChart1.SeriesMappings.Add(seriesMapping1);


            var seriesMapping2 = new SeriesMapping { LegendLabel = "Новым клиентам" };
            seriesMapping2.SeriesDefinition = new StackedBarSeriesDefinition();
            seriesMapping2.ItemMappings.Add(new ItemMapping("NewCustomers", DataPointMember.YValue));
            seriesMapping2.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory) { });
            amountTickets.SeriesMappings.Add(seriesMapping2);

            seriesMapping2 = new SeriesMapping { LegendLabel = "Старым клиентам" };
            seriesMapping2.SeriesDefinition = new StackedBarSeriesDefinition();
            seriesMapping2.ItemMappings.Add(new ItemMapping("OldCustomers", DataPointMember.YValue));
            seriesMapping2.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory) { });
            amountTickets.SeriesMappings.Add(seriesMapping2);


            pieChart.DefaultView.ChartLegend.Header = "";
            var seriesMapping = new SeriesMapping();
            seriesMapping.SeriesDefinition = new PieSeriesDefinition();
            seriesMapping.SeriesDefinition.LegendDisplayMode = LegendDisplayMode.DataPointLabel;
            seriesMapping.SeriesDefinition.LegendItemLabelFormat = "#XCAT";
            seriesMapping.ItemMappings.Add(new ItemMapping("CatName", DataPointMember.XCategory));
            seriesMapping.ItemMappings.Add(new ItemMapping("Value", DataPointMember.YValue) { });
            pieChart.SeriesMappings.Add(seriesMapping);

            amountTreatments.DefaultView.ChartLegend.Header = "";
            var seriesMappingAT = new SeriesMapping();
            seriesMappingAT.SeriesDefinition = new BarSeriesDefinition();
            seriesMappingAT.SeriesDefinition.LegendDisplayMode = LegendDisplayMode.DataPointLabel;
            seriesMappingAT.SeriesDefinition.LegendItemLabelFormat = "#XCAT";
            seriesMappingAT.ItemMappings.Add(new ItemMapping("CatName", DataPointMember.XCategory));
            seriesMappingAT.ItemMappings.Add(new ItemMapping("Value", DataPointMember.YValue) { });
            amountTreatments.SeriesMappings.Add(seriesMappingAT);

            visitChart.DefaultView.ChartLegend.Header = "";
            visitChart.DefaultView.ChartArea.AxisX.LayoutMode = AxisLayoutMode.Between;
            visitChart.DefaultView.ChartArea.AxisY.DefaultLabelFormat = "#VAL{N0}";


            Model.PropertyChanged += Model_PropertyChanged;
        }

        void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (e.PropertyName == "SalesDynamics")
                {
                    radChart.SeriesMappings.Clear();
                    if (Model.SalesDynamics.Any())
                    {
                        for (int i = 0; i < Model.SalesDynamics.First().Amount.Count; i++)
                        {
                            var seriesMapping = new SeriesMapping();
                            seriesMapping.LegendLabel = "Продажи " + (DateTime.Today.Year - i);
                            seriesMapping.SeriesDefinition = new LineSeriesDefinition();
                            seriesMapping.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory));
                            seriesMapping.ItemMappings.Add(new ItemMapping("Amount[" + i + "]", DataPointMember.YValue) { });
                            radChart.SeriesMappings.Add(seriesMapping);

                        }
                    }
                }

                if (e.PropertyName == "VisitsDynamics")
                {
                    visitChart.SeriesMappings.Clear();
                    if (Model.VisitsDynamics.Any())
                    {
                        for (int i = 0; i < Model.VisitsDynamics.First().Amount.Count; i++)
                        {
                            var seriesMapping = new SeriesMapping();
                            if (i == 0)
                            {
                                seriesMapping.LegendLabel = "На этой неделе";
                            }
                            else if (i == 1)
                            {
                                seriesMapping.LegendLabel = "На прошлой неделе";
                            }
                            else
                            {
                                seriesMapping.LegendLabel = "На позапрошлой неделе";
                            }
                            seriesMapping.SeriesDefinition = new BarSeriesDefinition();
                            seriesMapping.ItemMappings.Add(new ItemMapping("MonthName", DataPointMember.XCategory));
                            seriesMapping.ItemMappings.Add(new ItemMapping("Amount[" + i + "]", DataPointMember.YValue) { });
                            visitChart.SeriesMappings.Add(seriesMapping);

                        }
                    }
                }

            }));
        }
    }
}
