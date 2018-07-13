using System;
using NodaTime;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WeightInTime
    {
        public WeightInTime(Int64 unixTimeNanoseconds, Double weight)
        {
            Time = Instant.FromUnixTimeMilliseconds(unixTimeNanoseconds/1_000_000);
            Weight = weight;
        }

        public Instant Time { get; }
        public Double Weight { get; }
    }
}