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

namespace PL
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(
                nameof(Id),             // שם המאפיין
                typeof(int),            // סוג המאפיין
                typeof(Login),    // הבעלים של ה-DP
                new PropertyMetadata(0) // ערך ברירת מחדל
            );

        // Password DependencyProperty
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(Login),
                new PropertyMetadata(string.Empty)
            );


        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(BO.Role), typeof(Login), new PropertyMetadata(BO.Role.Volunteer));

        public BO.Role Role
        {
            get => (BO.Role)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }
        public Login()
        {
            InitializeComponent();
   #if DEBUG
            Id = 327773271;
            Password = "Eli Amar0556726282";
  #endif
        }
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Id == null || Id < 100000000 || Id > 999999999)
                {
                    MessageBox.Show("enter correct id");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("enter corrert password");
                    return;
                }

                Role = s_bl.Volunteer.Login(Id, Password);
                if (Role == BO.Role.Volunteer)
                {
                    new VolunteerWindow(Id, Role).Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mainWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(Id).Show();
            Close();
        }

        private void volunteerWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(Id).Show();
            Close();
        }
    }
}