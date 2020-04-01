using System;
using System.IO;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Taurit.Toolkit.WeightMonitor.GUI.Services
{
    public static class WallpaperSetter
    {
        private const Int32 SPI_SETDESKWALLPAPER = 20;
        private const Int32 SPIF_UPDATEINIFILE = 0x01;
        private const Int32 SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, String lpvParam, Int32 fuWinIni);


        public static void Set(String fileName)
        {
            String pathToImage = Path.Combine(Path.GetTempPath(), fileName);

            WallpaperSetter.SystemParametersInfo(WallpaperSetter.SPI_SETDESKWALLPAPER,
                0,
                pathToImage,
                WallpaperSetter.SPIF_UPDATEINIFILE | WallpaperSetter.SPIF_SENDWININICHANGE);
        }
    }
}