using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
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
using Color = System.Windows.Media.Color;

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


        private async Task LoadChartData()
        {
            foreach (BulkingPeriod bulkingPeriod in _settings.BulkingPeriods)
                AddReferenceLine(WeightChart, bulkingPeriod, Colors.LightSeaGreen);

            foreach (CuttingPeriod cuttingPeriod in _settings.CuttingPeriods)
                AddReferenceLine(WeightChart, cuttingPeriod, Colors.MediumVioletRed);

            var googleFitDataAccessor = new GoogleFitDataAccessor();
            WeightInTime[] weights = await googleFitDataAccessor.GetWeightDataPoints(_settings.NumPastDaysToShow);

            foreach (WeightInTime weight in weights)
                WeightData.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));
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



        private void GenerateAugmentedWallpaper(WallpaperConfiguration wallpaperSettings)
        {
            var originalWallpaper = new Bitmap(wallpaperSettings.BaseImagePath);
            BitmapFrame chart = VisualToBitmapConverter.Convert(WeightChart);

            // create overlay
            var finalImage = new Bitmap(1680, 1050);
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.DrawImage(originalWallpaper, new Rectangle(0, 0, 1680, 1050));
                g.DrawImage(BitmapFromSource(chart),
                    new Rectangle(wallpaperSettings.OffsetX, wallpaperSettings.OffsetY, 1680, 1050));
            }

            // save resulting file
            using (FileStream stream = File.Create(wallpaperSettings.FinalImagePath))
            {
                finalImage.Save(stream, ImageFormat.Jpeg);
            }
        }

        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        private async void MainWindow_OnLoaded(Object sender, RoutedEventArgs e)
        {
            await LoadChartData();

            GenerateAugmentedWallpaper(_settings.WallpaperToSet);

            WallpaperSetter.Set(_settings.WallpaperToSet.FinalImagePath);
        }
    }
}