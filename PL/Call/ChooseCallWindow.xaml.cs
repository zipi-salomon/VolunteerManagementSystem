using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int currentId { get; set; }
        public string UpdateAddress { get; set; } = "";
        public BO.CallType CallType { get; set; } = BO.CallType.None;
        public BO.OpenCallInListAttributes Attribute { get; set; } = BO.OpenCallInListAttributes.Id;

        public BO.OpenCallInList? SelectedCall
        {
            get { return (BO.OpenCallInList?)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(BO.OpenCallInList), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        public static readonly DependencyProperty OpenCallListProperty =
        DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public ChooseCallWindow(int id)
        {
            currentId = id;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            InitializeComponent();
        }

        private void filterBySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
           => queryOpenCallList();

        private void queryOpenCallList()
         => OpenCallList = (CallType == BO.CallType.None) ?
                s_bl?.Call.OpenCallsListSelectedByVolunteer(currentId, null, Attribute)! : s_bl?.Call.OpenCallsListSelectedByVolunteer(currentId, CallType, Attribute)!;

        //private void openCallListObserver()
        //   => queryOpenCallList();

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void openCallListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryOpenCallList();
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
          => s_bl.Call.AddObserver(openCallListObserver);

        private void Window_Closed(object sender, EventArgs e)
           => s_bl.Call.RemoveObserver(openCallListObserver);

        private void Choose_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to choose this call", "its ok?",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                try
                {
                    if (SelectedCall != null && CurrentVolunteer != null && CurrentVolunteer.CallingVolunteerTherapy == null)
                    {
                        s_bl.Call.ChooseTreatmentCall(CurrentVolunteer!.Id, SelectedCall!.Id);
                        MessageBox.Show($"choosen call succesfully:{CurrentVolunteer.CallingVolunteerTherapy}");
                    }
                    else
                        MessageBox.Show("you cant take another call, first finish with your current call");
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"error:{ex}");
                }
                catch (BO.BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }

        }
        private void UpdateAddress_click(object sender, RoutedEventArgs e)
        {
            try {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id,
                new BO.Volunteer()
                {
                    Id = CurrentVolunteer.Id,
                    Name = CurrentVolunteer.Name,
                    Phone = CurrentVolunteer.Phone,
                    Email = CurrentVolunteer.Email,
                    Role = CurrentVolunteer.Role,
                    Password = CurrentVolunteer.Password,
                    Active = CurrentVolunteer.Active,
                    Address = UpdateAddress,
                    Latitude = CurrentVolunteer.Latitude,
                    Longitude = CurrentVolunteer.Longitude,
                    MaxDistanceForCall = CurrentVolunteer.MaxDistanceForCall,
                    DistanceType = CurrentVolunteer.DistanceType,
                    TotalCallsCanceled = CurrentVolunteer.TotalCallsCanceled,
                    TotalCallsHandled = CurrentVolunteer.TotalCallsHandled,
                    TotalCallsChoseHandleHaveExpired = CurrentVolunteer.TotalCallsChoseHandleHaveExpired,
                    CallingVolunteerTherapy = CurrentVolunteer?.CallingVolunteerTherapy,

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            queryOpenCallList();
            
        }
        private void DisplayCallDescription(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"Description Call:{SelectedCall!.CallDescription}");
        }
    }
}