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
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.StatusCall StatusCall { get; set; } = BO.StatusCall.None;

        public CallWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            try
            {
                CurrentCall = id != 0
                    ? s_bl.Call.GetCallDetails(id)
                    :
                    new BO.Call() {CallType =BO.CallType.None, OpeningTime = s_bl.Admin.GetClock(), MaxTimeFinishCall = s_bl.Admin.GetClock(), StatusCall=BO.StatusCall.Open};
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Close();
            }
            StatusCall = CurrentCall!.StatusCall;
            InitializeComponent();
        }
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (CurrentCall == null) return;
                if (ButtonText == "Add")
                {
                    
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("call added successfully.");
                }
                else
                {
                    s_bl.Call.UpdateCallDetails(CurrentCall);
                    MessageBox.Show("call updated successfully.");
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        //private void CallObserver()
        //{
        //    int id = CurrentCall!.Id;
        //    CurrentCall = null;
        //    CurrentCall = s_bl.Call.GetCallDetails(id);
        //}
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void CallObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    int id = CurrentCall!.Id;
                    CurrentCall = null;
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.AddObserver(CurrentCall!.Id, CallObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
                s_bl.Call.RemoveObserver(CurrentCall!.Id, CallObserver);
        }
    }
}
