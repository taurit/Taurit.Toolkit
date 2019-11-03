using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WallpaperConfiguration
    {
        public String BaseImagePath { get; set; }
        public String FinalImagePath { get; set; }
        public Int32 OffsetX { get; set; }

        public Int32 OffsetY { get; set; }
        public Boolean GenerateWallpaperWithChart { get; set; }
    }
}