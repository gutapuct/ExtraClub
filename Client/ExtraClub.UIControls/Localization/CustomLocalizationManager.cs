using Telerik.Windows.Controls;

namespace ExtraClub.UIControls.Localization
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                case "GridViewGroupPanelText":
                    return UIControls.Localization.Resources.GridViewGroupPanelText;
                case "GridViewClearFilter":
                    return UIControls.Localization.Resources.GridViewClearFilter;
                case "GridViewFilterShowRowsWithValueThat":
                    return UIControls.Localization.Resources.GridViewFilterShowRowsWithValueThat;
                case "GridViewFilterSelectAll":
                    return UIControls.Localization.Resources.GridViewFilterSelectAll;
                case "GridViewFilterContains":
                    return UIControls.Localization.Resources.GridViewFilterContains;
                case "GridViewFilterEndsWith":
                    return UIControls.Localization.Resources.GridViewFilterEndsWith;
                case "GridViewFilterIsContainedIn":
                    return UIControls.Localization.Resources.GridViewFilterIsContainedIn;
                case "GridViewFilterIsEqualTo":
                    return UIControls.Localization.Resources.GridViewFilterIsEqualTo;
                case "GridViewFilterIsGreaterThan":
                    return UIControls.Localization.Resources.GridViewFilterIsGreaterThan;
                case "GridViewFilterIsGreaterThanOrEqualTo":
                    return UIControls.Localization.Resources.GridViewFilterIsGreaterThanOrEqualTo;
                case "GridViewFilterIsLessThan":
                    return UIControls.Localization.Resources.GridViewFilterIsLessThan;
                case "GridViewFilterIsLessThanOrEqualTo":
                    return UIControls.Localization.Resources.GridViewFilterIsLessThanOrEqualTo;
                case "GridViewFilterIsNotEqualTo":
                    return UIControls.Localization.Resources.GridViewFilterIsNotEqualTo;
                case "GridViewFilterStartsWith":
                    return UIControls.Localization.Resources.GridViewFilterStartsWith;
                case "GridViewFilterAnd":
                    return UIControls.Localization.Resources.GridViewFilterAnd;
                case "GridViewFilter":
                    return UIControls.Localization.Resources.GridViewFilter;
                case "GridViewFilterMatchCase":
                    return UIControls.Localization.Resources.GridViewFilterMatchCase;
                case "TileViewItemMinimizeText":
                    return UIControls.Localization.Resources.TileViewItemMinimizeText;
                case "TileViewItemMaximizeText":
                    return UIControls.Localization.Resources.TileViewItemMaximizeText;
                case "GridViewAlwaysVisibleNewRow":
                    return UIControls.Localization.Resources.GridViewAlwaysVisibleNewRow;
                case "GridViewGroupPanelTopTextGrouped":
                    return UIControls.Localization.Resources.GridViewGroupPanelTopTextGrouped;
                case "EnterDate":
                    return UIControls.Localization.Resources.EnterDate;
                case "Error":
                    return UIControls.Localization.Resources.Error;
                case "Close":
                    return UIControls.Localization.Resources.Close;
                case "Time":
                    return UIControls.Localization.Resources.Time;
                case "SetDayViewMode":
                    return UIControls.Localization.Resources.Day;
                case "SetWeekViewMode":
                    return UIControls.Localization.Resources.Week;
                case "DayHor":
                    return UIControls.Localization.Resources.DayHor;
                case "DayVer":
                    return UIControls.Localization.Resources.DayVer;
                case "Week":
                    return UIControls.Localization.Resources.Week;
                case "Clock":
                    return UIControls.Localization.Resources.Clock;
                case "GridViewFilterOr":
                    return UIControls.Localization.Resources.GridViewFilterOr;
                case "FilterMatchCase":
                    return UIControls.Localization.Resources.FilterMatchCase;
                case "EnterTime":
                    return UIControls.Localization.Resources.EnterTime;
                case "ChartChartLegendHeader":
                    return UIControls.Localization.Resources.Legend;
                case "ChartNoDataMessage":
                    return UIControls.Localization.Resources.ChartNoDataMessage;
                default:
                    break;
            }
            return base.GetStringOverride(key);
        }
    }
}
