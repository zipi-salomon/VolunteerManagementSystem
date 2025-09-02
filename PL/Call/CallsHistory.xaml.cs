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
    /// Interaction logic for CallsHistory.xaml
    /// </summary>
    public partial class CallsHistory : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int CurrentId { get; set; }
        public BO.CallType CallType { get; set; } = BO.CallType.None;
        public BO.ClosedCallInListAttributes Attribute { get; set; } = BO.ClosedCallInListAttributes.Id;
        public IEnumerable<BO.ClosedCallInList> ClosedCallsList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallsListProperty); }
            set { SetValue(ClosedCallsListProperty, value); }
        }

        public static readonly DependencyProperty ClosedCallsListProperty =
            DependencyProperty.Register("ClosedCallsList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallsHistory), new PropertyMetadata(null));

        public CallsHistory(int id)
        {
            CurrentId = id;
            InitializeComponent();
        }
        private void filterBySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
           => queryClosedCallList();

        private void queryClosedCallList()
         => ClosedCallsList = (CallType == BO.CallType.None) ?
                s_bl?.Call.ClosedCallsListHandledByVolunteer(CurrentId, null, Attribute)! : s_bl?.Call.ClosedCallsListHandledByVolunteer(CurrentId, CallType, Attribute)!;

        //private void ClosedCallListObserver()
        //   => queryClosedCallList();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void ClosedCallListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryClosedCallList();
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
          => s_bl.Call.AddObserver(ClosedCallListObserver);

        private void Window_Closed(object sender, EventArgs e)
           => s_bl.Call.RemoveObserver(ClosedCallListObserver);

    }
}
