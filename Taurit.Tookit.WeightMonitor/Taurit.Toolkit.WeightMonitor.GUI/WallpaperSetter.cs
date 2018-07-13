using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    public sealed class WallpaperSetter
    {
        private const Int32 SPI_SETDESKWALLPAPER = 20;
        private const Int32 SPIF_UPDATEINIFILE = 0x01;
        private const Int32 SPIF_SENDWININICHANGE = 0x02;

        private WallpaperSetter()
        {
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, String lpvParam, Int32 fuWinIni);


        public static void Set(String fileName)
        {
            String pathToImage = Path.Combine(Path.GetTempPath(), fileName);

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                pathToImage,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}