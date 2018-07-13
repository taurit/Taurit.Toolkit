﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using LiveCharts.Defaults;
using Taurit.Toolkit.WeightMonitor.Common.Models;
using Taurit.Toolkit.WeightMonitor.GUI.Services;
using Taurit.Toolkit.WeightMonitor.GUI.Xaml2009Workarounds;

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

        [NotNull]
        public ChartValuesDateTimePoint WeightData { get; } = new ChartValuesDateTimePoint();

        private async void Button_Click(Object sender, RoutedEventArgs e)
        {
            var googleFitDataAccessor = new GoogleFitDataAccessor();
            WeightInTime[] weights = await googleFitDataAccessor.GetWeightDataPoints(2*365);

            foreach (WeightInTime weight in weights)
                WeightData.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));

            //var wallpaperFileName = "d:\\chart.png";
            //SaveToPng(xchart, wallpaperFileName);
            //WallpaperSetter.Set(wallpaperFileName);
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