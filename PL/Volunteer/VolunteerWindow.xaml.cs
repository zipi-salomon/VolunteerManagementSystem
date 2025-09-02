using BO;
using PL.Call;
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
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerWindow(int id = 0, BO.Role role = BO.Role.Manager)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            Role = role;
            InitializeComponent();

            try
            {
                CurrentVolunteer = id != 0
                    ? s_bl.Volunteer.GetVolunteerDetails(id)
                    :
                    new BO.Volunteer() { Id = 0, Role = BO.Role.Volunteer, Active = false };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Close();
            }
        }
        public BO.Role Role { get; set; }
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private bool formatCheck()
        {
            if (CurrentVolunteer?.Id.ToString().Length != 9)
            {
                MessageBox.Show("id should be 9 number long");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.Name))
            {
                MessageBox.Show("Name cannot be empty");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.Phone) || CurrentVolunteer.Phone.ToString().Length != 10)
            {
                MessageBox.Show("Phone number must be exactly 10 digits");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.Email) || !CurrentVolunteer.Email.Contains("@"))
            {
                MessageBox.Show("Invalid email format");
                return false;
            }
            if (CurrentVolunteer.MaxDistanceForCall < 0)
            {
                MessageBox.Show("Maximum distance must be non-negative");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.Address))
            {
                MessageBox.Show("Address cannot be empty");
                return false;
            }

            return true;
        }
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (CurrentVolunteer == null) return;
                if (formatCheck())
                    if (ButtonText == "Add")
                    {
                        s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                        MessageBox.Show("volunteer added successfully.");
                    }
                    else
                    {
                        s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                        MessageBox.Show("volunteer updated successfully.");
                    }
                Close();
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            catch (BO.BlUnauthorizedException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Close();
            }
        }




        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void VolunteerObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentVolunteer!.Id;
                    CurrentVolunteer = null;
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                });
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
        }

        private void ChooseCall_Btn_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallWindow(CurrentVolunteer!.Id).Show();
        }

        private void callHistory_Btn_Click(object sender, RoutedEventArgs e)
        {
            new CallsHistory(CurrentVolunteer!.Id).Show();
        }

        private void cancelTreatmentBtn_click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to cancel the assignment of this call", "its ok?",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                if (CurrentVolunteer.CallingVolunteerTherapy.Id != null)
                    try
                    {
                        s_bl.Call.UpdateCancelTreatmentOnCall(CurrentVolunteer.Id, CurrentVolunteer.CallingVolunteerTherapy.Id);
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
                    MessageBox.Show("you dony have a call now");
                }
            }
        }

        private void endTreatmentBtn_click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageResult = MessageBox.Show("Are you sure you want to finish treatment at this call", "its ok?",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Information);
            if (messageResult == MessageBoxResult.OK)
            {
                if (CurrentVolunteer.CallingVolunteerTherapy.Id != null)
                    try
                    {
                        s_bl.Call.UpdateEndTreatmentOnCall(CurrentVolunteer.Id, CurrentVolunteer.CallingVolunteerTherapy.Id);
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't update finish this call treatment");
                    }
                    catch (BO.BlCantUpdateException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't update finish this call treatment");
                    }
                    catch (BO.BlUnauthorizedException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't update finish this call treatment");
                    }
                    catch (BO.BlInvalidValueException ex)
                    {
                        MessageBox.Show(ex.Message, "Can't update with invalid value");
                    }
                    catch (BO.BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                else
                {
                    MessageBox.Show("you d'ont have a call now");
                }
            }
        }
    }
}