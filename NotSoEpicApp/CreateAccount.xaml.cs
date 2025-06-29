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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotSoEpicApp
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Page
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }


        private async void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            // Validate username
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || UsernameTextBox.Text.Length < 5)
            {
                string message = Application.Current.FindResource("UsernameValidation") as string;
                MessageBox.Show(message);
                return;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
            {
                string message = Application.Current.FindResource("EmailValidation") as string;
                MessageBox.Show(message);
                return;
            }

            // Validate password
            if (PasswordBox.Password.Length < 8)
            {
                string message = Application.Current.FindResource("PasswordValidation") as string;
                MessageBox.Show(message);
                return;
            }

            // Validate password confirmation
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                string message = Application.Current.FindResource("PasswordConfirmationValidation") as string;
                MessageBox.Show(message);
                return;
            }

            //  create the user
            byte[] hashedPassword = PasswordManager.HashPassword(PasswordBox.Password);

            user newUser = new user
            {
                Name = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                PasswordHash = hashedPassword
            };
            bool success = await Database.SaveUserToDatabase(newUser);

            if (success)
            {
                string message = Application.Current.FindResource("AccountCreatedSuccess") as string;
                MessageBox.Show(message);
                NavigationService.Navigate(new Login());
            }
            else
            {
                string message = Application.Current.FindResource("AccountCreatedFail") as string;
                MessageBox.Show(message);
            }



            UsernameTextBox.Clear();
            EmailTextBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
