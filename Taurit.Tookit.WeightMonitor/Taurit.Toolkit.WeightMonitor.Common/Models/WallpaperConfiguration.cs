using System;

namespace Taurit.Toolkit.WeightMonitor.Common.Models
{
    public class WallpaperConfiguration
    {
        public String BaseImagePath { get; set; }
        public String FinalImagePath { get; set; }
        
        public Int32? OffsetX { get; set; }
        public Int32? OffsetY { get; set; }
        public Boolean OffsetIsProvided => OffsetX != null && OffsetX >= 0 && OffsetY != null && OffsetY >= 0;

        public Boolean GenerateWallpaperWithChart { get; set; }


    }
}