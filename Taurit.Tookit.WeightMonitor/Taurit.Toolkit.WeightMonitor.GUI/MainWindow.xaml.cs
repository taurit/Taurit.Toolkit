using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using NodaTime;
using Taurit.Toolkit.WeightMonitor.Common.Models;
using Taurit.Toolkit.WeightMonitor.GUI.Services;
using Duration = NodaTime.Duration;

namespace Taurit.Toolkit.WeightMonitor.GUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow
    {
        private readonly WeightMonitorSettings _settings;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                var settingsFilesToProbe = new[]
                {
                    "WeightMonitor.json",
                    "d:\\ProgramData\\ApplicationData\\TauritToolkit\\WeightMonitor.jsonc"
                };

                String settingsFilePath = settingsFilesToProbe.First(File.Exists);

                String settingsAsJson = File.ReadAllText(settingsFilePath);
                _settings = JsonConvert.DeserializeObject<WeightMonitorSettings>(settingsAsJson);

                // poor man's injection
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public Func<Double, String> XFormatter { get; } = value => new DateTime((Int64) value).ToString("yyyy-MM-dd");

        [NotNull]
        public ChartValues<DateTimePoint> WeightData { get; } = new ChartValues<DateTimePoint>();


        private async Task LoadChartData()
        {
            BulkingPeriod[] bulkingPeriods = _settings.ShowPastPeriods
                ? _settings.BulkingPeriods
                : _settings.BulkingPeriods.Where(x => x.End > DateTime.Today).ToArray();

            MaintenancePeriod[] maintenancePeriods = _settings.ShowPastPeriods
                ? _settings.MaintenancePeriods
                : _settings.MaintenancePeriods.Where(x => x.End > DateTime.Today).ToArray();

            CuttingPeriod[] cuttingPeriods = _settings.ShowPastPeriods
                ? _settings.CuttingPeriods
                : _settings.CuttingPeriods.Where(x => x.End > DateTime.Today).ToArray();

            foreach (CuttingPeriod cuttingPeriod in cuttingPeriods)
                MainWindow.AddReferenceLine(WeightChart, cuttingPeriod, Colors.PaleVioletRed, _settings.NumFutureDaysToShow);
            foreach (MaintenancePeriod maintenancePeriod in maintenancePeriods)
                MainWindow.AddReferenceLine(WeightChart, maintenancePeriod, Colors.Orange, _settings.NumFutureDaysToShow);
            foreach (BulkingPeriod bulkingPeriod in bulkingPeriods)
                MainWindow.AddReferenceLine(WeightChart, bulkingPeriod, Colors.LightGreen, _settings.NumFutureDaysToShow);

            var googleFitDataAccessor = new GoogleFitDataAccessor();
            WeightInTime[] weights = await googleFitDataAccessor.GetWeightDataPoints(_settings.NumPastDaysToShow);

            if (weights.Length == 0)
                MessageBox.Show($"No weight data found in Google for the last {_settings.NumPastDaysToShow}");

            List<DateTimePoint> allWeights = MainWindow.GetPointsForChart(weights, false);

            // in case there are many data point in a day, compute daily average
            foreach (IGrouping<DateTime, DateTimePoint> weightsGroupedByDate in allWeights.GroupBy(x => x.DateTime.Date)
            ) WeightData.Add(new DateTimePoint(weightsGroupedByDate.Key, weightsGroupedByDate.Average(y => y.Value)));
        }

        private static List<DateTimePoint> GetPointsForChart(WeightInTime[] weights, bool skipConsecutiveMeasurementsWithTheSameValue)
        {
            var allWeights = new List<DateTimePoint>(weights.Length);

            Double lastWeight = 0;
            foreach (WeightInTime weight in weights)
            {
                // There used to be a problem somewhere in Mi Weight or Mi Fit or Google Fit that
                // makes the weight stored multiple times with usual intervals of 8h (failing retry mechanism?)
                // this condition is to remove such wrong from the graph.
                //
                // As of 2020-04-01 I no longer observe it in the recent data.
                //
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!skipConsecutiveMeasurementsWithTheSameValue || weight.Weight != lastWeight)
                {
                    lastWeight = weight.Weight;
                    allWeights.Add(new DateTimePoint(weight.Time.ToDateTimeUtc(), weight.Weight));
                }
            }

            return allWeights;
        }

        private static void AddReferenceLine(
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
            trainingPeriod.Trim(endDateToDraw);

            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<DateTimePoint>
                {
                    new DateTimePoint(trainingPeriod.Start, trainingPeriod.StartWeight),
                    new DateTimePoint(trainingPeriod.End, trainingPeriod.ExpectedMinimumEndWeight)
                },
                Stroke = new SolidColorBrush(lineColor),
                Fill = Brushes.Transparent
            });

            chart.Series.Add(new LineSeries
            {
                Values = new ChartValues<DateTimePoint>
                {
                    new DateTimePoint(trainingPeriod.Start, trainingPeriod.StartWeight),
                    new DateTimePoint(trainingPeriod.End, trainingPeriod.ExpectedMaximumEndWeight)
                },
                Stroke = new SolidColorBrush(lineColor),
                Fill = Brushes.Transparent
            });
        }

        private async void MainWindow_OnContentRendered(Object sender, EventArgs e)
        {
            try
            {
                await LoadChartData();

                if (_settings.WallpaperToSet.GenerateWallpaperWithChart)
                {
                    await Task.Delay(2000); // workaround for chart not yet rendered - I'm not sure why
                    WallpaperGenerator.GenerateAugmentedWallpaper(_settings.WallpaperToSet, ChartWrapper);

                    WallpaperSetter.Set(_settings.WallpaperToSet.FinalImagePath);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}