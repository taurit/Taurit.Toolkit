using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class TimePeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Int32 DurationInDays => (End - Start).Days;
    }
}