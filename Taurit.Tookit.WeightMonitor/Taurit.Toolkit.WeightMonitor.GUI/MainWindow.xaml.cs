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
using NodaTime;
using Taurit.Toolkit.WeightMonitor.Common.Models;
using Taurit.Toolkit.WeightMonitor.GUI.Services;
using Color = System.Windows.Media.Color;
using Duration = NodaTime.Duration;

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
                AddReferenceLine(WeightChart, bulkingPeriod, Colors.LightSeaGreen, _settings.NumFutureDaysToShow);

            foreach (CuttingPeriod cuttingPeriod in _settings.CuttingPeriods)
                AddReferenceLine(WeightChart, cuttingPeriod, Colors.MediumVioletRed, _settings.NumFutureDaysToShow);

            var googleFitDataAccessor = new GoogleFitDataAccessor();
            WeightInTime[] weights = await googleFitDataAccessor.GetWeightDataPoints(_settings.NumPastDaysToShow);

            var lastWeight = 0d;
            foreach (WeightInTime weight in weights)
            {
                // there seem to be a problem somewhere in Mi Weight or Mi Fit or Google Fit that
                // makes the weight stored multiple times with usual intervals of 8h (failing retry mechanism?)
                // this condition is to remove such wrong from the graph
                if (weight.Weight != lastWeight)
                {
                    WeightData.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));
                }
                
                lastWeight = weight.Weight;
            }
                
        }

        private void AddReferenceLine(
            [NotNull] CartesianChart chart,
            [NotNull] TrainingPeriod trainingPeriod,
            Color lineColor,
            Int32 numFutureDaysToShow)
        {
            // make sure that we don't draw beyond the max future date to display
            Duration duration = Duration.FromDays(numFutureDaysToShow);
            DateTime maxDate = SystemClock.Instance.GetCurrentInstant().Plus(duration).ToDateTimeUtc();

            if (trainingPeriod.Start >= maxDate) return;

            DateTime endDateToDraw = trainingPeriod.End >= maxDate ? maxDate : trainingPeriod.End;

            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<DateTimePoint>
                {
                    new DateTimePoint(trainingPeriod.Start, trainingPeriod.StartWeight),
                    new DateTimePoint(endDateToDraw, trainingPeriod.ExpectedEndWeight)
                },
                Stroke = new SolidColorBrush(lineColor)
            });
        }


        private void GenerateAugmentedWallpaper(WallpaperConfiguration wallpaperSettings)
        {
            var originalWallpaper = new Bitmap(wallpaperSettings.BaseImagePath);
            BitmapFrame chart = VisualToBitmapConverter.Convert(ChartWrapper);

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


        private async void MainWindow_OnContentRendered(Object sender, EventArgs e)
        {
            await LoadChartData();

            await Task.Delay(2000); // workaround for chart not yet rendered - I'm not sure why
            GenerateAugmentedWallpaper(_settings.WallpaperToSet);

            WallpaperSetter.Set(_settings.WallpaperToSet.FinalImagePath);
            Application.Current.Shutdown();
        }
    }
}