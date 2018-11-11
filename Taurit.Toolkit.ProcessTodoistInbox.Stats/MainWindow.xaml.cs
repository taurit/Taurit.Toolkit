using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using LiveCharts;
using LiveCharts.Defaults;

namespace Taurit.Toolkit.ProcessTodoistInbox.Stats
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StatsAppSettings _settings;

        public MainWindow()
        {
            InitializeComponent();
            _settings = new StatsAppSettings();
        }

        public Func<Double, String> XFormatter { get; } = value =>
        {
            return new DateTime((Int64) value).ToString("yyyy-MM-dd");
        };

        [NotNull]
        public ChartValues<DateTimePoint> YData { get; } = new ChartValues<DateTimePoint>();

        private void RadioButtonSetupChanged([CanBeNull] Object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            YData.Clear();

            TimeSpan selectedTimePeriod = GetSelectedTimePeriod();


            YData.Add(new DateTimePoint(DateTime.Now, 666));
            YData.Add(new DateTimePoint(DateTime.Now.AddDays(1), 666));
            YData.Add(new DateTimePoint(DateTime.Now.AddDays(3), 766));
        }

        private TimeSpan GetSelectedTimePeriod()
        {
            RadioButton[] timePeriodCheckboxes =
            {
                Time_AllTime,
                Time_LastMonth,
                Time_LastWeek,
                Time_LastYear
            };
            RadioButton selectedTimeCheckbox = timePeriodCheckboxes.Single(x => x.IsChecked == true);
            Int32 timeInDays = Convert.ToInt32((String) selectedTimeCheckbox.Tag);
            return TimeSpan.FromDays(timeInDays);
        }


        private void MainWindow_OnContentRendered(Object sender, EventArgs e)
        {
            RadioButtonSetupChanged(null, null);
        }
    }
}