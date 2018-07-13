using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Taurit.Toolkit.WeightMonitor.Common.Models;
using Taurit.Toolkit.WeightMonitor.GUI.Services;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WeightMonitorSettings _settings;

        public MainWindow()
        {
            InitializeComponent();
            var settingsFilePath = "d:\\ProgramData\\ApplicationData\\TauritToolkit\\WeightMonitor.json";
            String settingsAsJson = File.ReadAllText(settingsFilePath);
            _settings = JsonConvert.DeserializeObject<WeightMonitorSettings>(settingsAsJson);
        }

        public Func<Double, String> XFormatter { get; } = value =>
        {
            return new DateTime((Int64) value).ToString("yyyy-MM-dd");
        };

        [NotNull]
        public ChartValues<DateTimePoint> WeightData { get; } = new ChartValues<DateTimePoint>();

        private async void LoadRealData_Click(Object sender, RoutedEventArgs e)
        {
            foreach (BulkingPeriod bulkingPeriod in _settings.BulkingPeriods)
                AddReferenceLine(WeightChart, bulkingPeriod, Colors.LightSeaGreen);

            foreach (CuttingPeriod cuttingPeriod in _settings.CuttingPeriods)
                AddReferenceLine(WeightChart, cuttingPeriod, Colors.MediumVioletRed);

            var googleFitDataAccessor = new GoogleFitDataAccessor();
            WeightInTime[] weights = await googleFitDataAccessor.GetWeightDataPoints(_settings.NumPastDaysToShow);

            foreach (WeightInTime weight in weights)
                WeightData.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));


            //var wallpaperFileName = "d:\\chart.png";
            //SaveToPng(WeightChart, wallpaperFileName);
            //WallpaperSetter.Set(wallpaperFileName);
        }

        private void AddReferenceLine(
            [NotNull] CartesianChart chart,
            [NotNull] TrainingPeriod trainingPeriod,
            Color lineColor)
        {
            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<DateTimePoint>
                {
                    new DateTimePoint(trainingPeriod.Start, trainingPeriod.StartWeight),
                    new DateTimePoint(trainingPeriod.End, trainingPeriod.ExpectedEndWeight)
                },
                Stroke = new SolidColorBrush(lineColor)
            });
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