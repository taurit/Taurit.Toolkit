using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
    public partial class MainWindow
    {
        private readonly WeightMonitorSettings _settings;
        private readonly WallpaperGenerator _wallpaperGenerator;

        public MainWindow()
        {
            InitializeComponent();
            var settingsFilePath = "d:\\ProgramData\\ApplicationData\\TauritToolkit\\WeightMonitor.json";
            String settingsAsJson = File.ReadAllText(settingsFilePath);
            _settings = JsonConvert.DeserializeObject<WeightMonitorSettings>(settingsAsJson);

            // poor man's injection
            _wallpaperGenerator = new WallpaperGenerator();
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");

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

            if (weights.Length == 0)
            {
                MessageBox.Show($"No weight data found in Google for the last {_settings.NumPastDaysToShow}");
            }

            var allWeights = new List<DateTimePoint>(weights.Length);
            Double lastWeight = 0;
            foreach (WeightInTime weight in weights)
            {
                // there seem to be a problem somewhere in Mi Weight or Mi Fit or Google Fit that
                // makes the weight stored multiple times with usual intervals of 8h (failing retry mechanism?)
                // this condition is to remove such wrong from the graph
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (weight.Weight != lastWeight)
                    allWeights.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));
                lastWeight = weight.Weight;
            }


            // in case there are many data point in a day, compute daily average
            foreach (IGrouping<DateTime, DateTimePoint> weightsGroupedByDate in allWeights.GroupBy(x => x.DateTime.Date)
            ) WeightData.Add(new DateTimePoint(weightsGroupedByDate.Key, weightsGroupedByDate.Average(y => y.Value)));
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

        private async void MainWindow_OnContentRendered(Object sender, EventArgs e)
        {
            await LoadChartData();

            if (_settings.WallpaperToSet.GenerateWallpaperWithChart)
            {
                await Task.Delay(2000); // workaround for chart not yet rendered - I'm not sure why
                _wallpaperGenerator.GenerateAugmentedWallpaper(_settings.WallpaperToSet, ChartWrapper);

                WallpaperSetter.Set(_settings.WallpaperToSet.FinalImagePath);
                Application.Current.Shutdown();
            }
            
        }
    }
}