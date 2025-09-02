using PL.Call;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int Id { get; }

        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register(nameof(IsSimulatorRunning), typeof(bool), typeof(MainWindow));

        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(int), typeof(MainWindow));
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
       DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan CurrentRiskRange
        {
            get { return (TimeSpan)GetValue(CurrentRiskRangeProperty); }
            set { SetValue(CurrentRiskRangeProperty, value); }
        }
        public static readonly DependencyProperty CurrentRiskRangeProperty =
        DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow));

        public int[] CallByStatus
        {
            get { return (int[])GetValue(CallByStatusProperty); }
            set { SetValue(CallByStatusProperty, value); }
        }

        public static readonly DependencyProperty CallByStatusProperty =
            DependencyProperty.Register("CallByStatus", typeof(int[]), typeof(MainWindow));

        public MainWindow(int id)
        {
            Interval = 1;
            CallByStatus = s_bl.Call.GetCallQuantitiesByStatus();
            Id = id;
            InitializeComponent();
        }

        private void SimulatorToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }
    
        private volatile DispatcherOperation? _observerOperationClock = null; //stage 7
        private volatile DispatcherOperation? _observerOperationConfig = null; //stage 7
        private volatile DispatcherOperation? _observerOperationCallByStatus = null; //stage 7

        private void clockObserver() //stage 7
        {
            if (_observerOperationClock is null || _observerOperationClock.Status == DispatcherOperationStatus.Completed)
                _observerOperationClock = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }

        private void configObserver() //stage 7
        {
            if (_observerOperationConfig is null || _observerOperationConfig.Status == DispatcherOperationStatus.Completed)
                _observerOperationConfig = Dispatcher.BeginInvoke(() =>
                {
                    CurrentRiskRange = s_bl.Admin.GetRiskRange();
                });
        }
        private void callByStatusObserver() //stage 7
        {
            if (_observerOperationCallByStatus is null || _observerOperationCallByStatus.Status == DispatcherOperationStatus.Completed)
                _observerOperationCallByStatus = Dispatcher.BeginInvoke(() =>
                {
                    CallByStatus = s_bl.Call.GetCallQuantitiesByStatus();
                });
        }

        //functions to advnce time
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);
        }

        private void btnUpdateRiskRange_click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(CurrentRiskRange);
            MessageBox.Show("succeful update risk range!");

        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            CurrentRiskRange = s_bl.Admin.GetRiskRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(callByStatusObserver);
        }

        private void window_Closed(object sender, EventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }

            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(callByStatusObserver);
        }

        private void showCallsByStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string roleTag)
            {
                new CallListWindow(Id, (BO.StatusCall)Enum.Parse(typeof(BO.StatusCall), roleTag)).Show();
            }
            else
            {
                new CallListWindow(Id).Show();
            }
        }
        private void btnViewVolunteerList_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }
        private void btnViewCallList_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(Id).Show();
        }

        private void resetDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to reset?", "its ok?",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                s_bl.Admin.ResetDB();
                Mouse.OverrideCursor = null;

            }

        }
        private void initDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to init?", "its ok?",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {   //Changes the mouse to an hourglass shape.
                Mouse.OverrideCursor = Cursors.Wait;
                s_bl.Admin.InitializeDB();
                Mouse.OverrideCursor = null;
            }

        }
    }
}