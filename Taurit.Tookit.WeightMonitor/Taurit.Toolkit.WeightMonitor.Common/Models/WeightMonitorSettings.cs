using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WeightMonitorSettings
    {
        public Int32 NumPastDaysToShow { get; set; }
        public Int32 NumFutureDaysToShow { get; set; }
        public Boolean ShowPastPeriods { get; set; }
        public WallpaperConfiguration WallpaperToSet { get; set; }
        public BulkingPeriod[] BulkingPeriods { get; set; }
        public MaintenancePeriod[] MaintenancePeriods { get; set; }
        public CuttingPeriod[] CuttingPeriods { get; set; }
    }
}