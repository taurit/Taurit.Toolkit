using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Wpf;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Func<Double, String> XFormatter { get; } = value =>
        {
            return new DateTime((Int64) value).ToString("yyyy-MM-dd");
        };

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            var chart = new CartesianChart
            {
                DisableAnimations = true,
                Width = 600,
                Height = 200,
                Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Values = new ChartValues<Double> {1, 6, 7, 2, 9, 3, 6, 5}
                    }
                }
            };

            var viewbox = new Viewbox();
            viewbox.Child = chart;
            viewbox.Measure(chart.RenderSize);
            viewbox.Arrange(new Rect(new Point(0, 0), chart.RenderSize));
            chart.Update(true, true); //force chart redraw
            viewbox.UpdateLayout();


            var wallpaperFileName = "d:\\chart.png";
            SaveToPng(xchart, wallpaperFileName);
            //png file was created at the root directory.
            WallpaperSetter.Set(wallpaperFileName);
        }

        private void SaveToPng(FrameworkElement visual, String fileName)
        {
            var encoder = new PngBitmapEncoder();
            EncodeVisual(visual, fileName, encoder);
        }

        private static void EncodeVisual(FrameworkElement visual, String fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((Int32) visual.ActualWidth, (Int32) visual.ActualHeight, 96, 96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (FileStream stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
    }
}