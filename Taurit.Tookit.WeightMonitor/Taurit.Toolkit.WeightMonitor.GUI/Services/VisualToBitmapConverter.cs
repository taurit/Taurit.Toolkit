using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace Taurit.Toolkit.WeightMonitor.GUI.Services
{
    internal class VisualToBitmapConverter
    {
        public static BitmapFrame Convert([NotNull] FrameworkElement visual)
        {
            var bitmap = new RenderTargetBitmap((Int32) visual.ActualWidth, (Int32) visual.ActualHeight, 96, 96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            return frame;
        }
    }
}