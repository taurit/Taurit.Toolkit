using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Taurit.Toolkit.WeightMonitor.Common.Models;

namespace Taurit.Toolkit.WeightMonitor.GUI.Services
{
    /// <summary>
    ///     Allows to add an overlay to a wallpaper with a weight chart and to save back such modified wallpaper to a disk
    /// </summary>
    internal class WallpaperGenerator
    {
        public void GenerateAugmentedWallpaper(WallpaperConfiguration wallpaperSettings, Grid chartWrapper)
        {
            var originalWallpaper = new Bitmap(wallpaperSettings.BaseImagePath);
            BitmapFrame chart = VisualToBitmapConverter.Convert(chartWrapper);

            // create overlay
            var finalImage = new Bitmap(1680, 1050);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.DrawImage(originalWallpaper, new Rectangle(0, 0, 1680, 1050));
                Bitmap chartBitmap = BitmapFromSource(chart);
                g.DrawImage(chartBitmap,
                    new Rectangle(wallpaperSettings.OffsetX, wallpaperSettings.OffsetY, chartBitmap.Width,
                        chartBitmap.Height));
            }

            // save resulting file
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            Encoder qualityEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(qualityEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            finalImage.Save(wallpaperSettings.FinalImagePath, jgpEncoder, myEncoderParameters);
        }

        private Bitmap BitmapFromSource(BitmapSource bitmapSource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }

            return null;
        }
    }
}