using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WeightMonitorSettings
    {
        public Int32 NumPastDaysToShow { get; set; }
        public WallpaperConfiguration WallpaperToSet { get; set; }
        public BulkingPeriod[] BulkingPeriods { get; set; }
        public CuttingPeriod[] CuttingPeriods { get; set; }
    }
}