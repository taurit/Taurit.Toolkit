using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WallpaperConfiguration
    {
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }
        public String BackgroundColor { get; set; }

        public Int32? OffsetX { get; set; }
        public Int32? OffsetY { get; set; }
        public Boolean OffsetIsProvided => OffsetX != null && OffsetX >= 0 && OffsetY != null && OffsetY >= 0;

        public Boolean GenerateWallpaperWithChart { get; set; }

        public String FinalImagePath { get; set; }
    }
}