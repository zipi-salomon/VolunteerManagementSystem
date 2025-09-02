using PL.Volunteer;
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
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.StatusCall Status { get; set; } = BO.StatusCall.None;

        public int IdRequest { get; }
        public BO.CallInList? SelectedCall { get; set; }

        public BO.CallType CallType { get; set; } = BO.CallType.None;
        public BO.CallInListAttributes Attribute { get; set; } = BO.CallInListAttributes.Id;

        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public CallListWindow(int id, BO.StatusCall statusTofilter = BO.StatusCall.None)
        {
            Status = statusTofilter;
            IdRequest = id;
            InitializeComponent();
        }
        private void filterListByStatus(object sender, SelectionChangedEventArgs e)
          =>
           queryCallList();
        private void filterBySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
           => queryCallList();

        private void queryCallList()
        {
            CallList = (CallType == BO.CallType.None) ?
                s_bl?.Call.GetCallsList(null, null, Attribute)! : s_bl?.Call.GetCallsList(BO.CallInListAttributes.CallType, CallType, Attribute)!;
            CallList = (Status == BO.StatusCall.None) ?
               CallList : CallList.Where(c => c.StatusCall == Status);
        }

        //private void callListObserver()
        //   => queryCallList();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void callListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallList();
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
          => s_bl.Call.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
           => s_bl.Call.RemoveObserver(callListObserver);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }
        private void callsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var s = SelectedCall;
            if (SelectedCall?.CallId != null)
                new CallWindow(SelectedCall.CallId).Show();
        }
        private void delete_btnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to delete the call", "its ok?",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                if (SelectedCall != null)
                    try
                    {
                        s_bl.Call.DeleteCall(SelectedCall.CallId);
                    }
                    catch (BO.BlCantDeleteException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't delete thr call");
                    }
                    catch (BO.BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
            }
        }

        private void CancelTreatCall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to cancel the assignment of this call", "its ok?",
           MessageBoxButton.OKCancel,
           MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                if (SelectedCall.Id != null)
                    try
                    {
                        s_bl.Call.UpdateCancelTreatmentOnCall(IdRequest, SelectedCall.Id.Value);
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't cancel the assignment");
                    }
                    catch (BO.BlUnauthorizedException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't cancel the assignment");
                    }
                    catch (BO.BlCantUpdateException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't cancel the assignment");
                    }
                    catch (BO.BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                else
                {
                    MessageBox.Show("this call is open now");
                }
            }
        }
    }
}

