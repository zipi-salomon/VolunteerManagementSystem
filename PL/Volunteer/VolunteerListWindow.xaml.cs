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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public BO.CallType CallType { get; set; } = BO.CallType.None;
        public BO.VolunteerInListAttributes Attribute { get; set; } = BO.VolunteerInListAttributes.Id;
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        private void filterBySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
           => queryVolunteerList();

        private void queryVolunteerList()
         => VolunteerList = (CallType == BO.CallType.None) ?
                s_bl?.Volunteer.GetVolunteersList(null,null, Attribute)! : s_bl?.Volunteer.GetVolunteersList(BO.VolunteerInListAttributes.CallType, CallType, Attribute)!;

        //private void volunteerListObserver()
        //   => queryVolunteerList();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void volunteerListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
          => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
           => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
        private void volunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();

        }
        
        private void delete_btnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the volunteer", "its ok?",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                if (SelectedVolunteer != null)
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer.Id);
                    }
                    catch (BO.BlCantDeleteException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't delete the volunteer");
                    }
                    catch (BO.BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
            }
        }
    }
}