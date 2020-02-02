using System;
using System.Diagnostics;
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
            BitmapFrame chart = VisualToBitmapConverter.Convert(chartWrapper);

            // create overlay
            var wallpaperSize = new Size(wallpaperSettings.Width, wallpaperSettings.Height);

            var finalImage = new Bitmap(wallpaperSize.Width, wallpaperSize.Height);
            using Graphics g = Graphics.FromImage(finalImage);
            using var brush = new SolidBrush(ColorTranslator.FromHtml(wallpaperSettings.BackgroundColor));
            g.FillRectangle(brush, 0, 0, wallpaperSize.Width, wallpaperSize.Height);
            Bitmap chartBitmap = BitmapFromSource(chart);

            // center image be default 
            Int32 offsetX = (wallpaperSize.Width - chartBitmap.Width) / 2;
            Int32 offsetY = (wallpaperSize.Height - chartBitmap.Height) / 2;
                
            if (wallpaperSettings.OffsetIsProvided)
            {
                Debug.Assert(wallpaperSettings.OffsetX != null, "wallpaperSettings.OffsetX != null");
                Debug.Assert(wallpaperSettings.OffsetY != null, "wallpaperSettings.OffsetY != null");

                offsetX = wallpaperSettings.OffsetX.Value;
                offsetY = wallpaperSettings.OffsetY.Value;
            }

            g.DrawImage(chartBitmap, new Rectangle(offsetX, offsetY, chartBitmap.Width, chartBitmap.Height));

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