using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public static List<Transaction> transactions;
        public static int? CurrentUserId { get;  set; }
        public static decimal? Current_money;

        public static SupervisorInfo CurrentPermissions { get; set; } = new SupervisorInfo
        {
            AllowViewCharts = true,
            AllowViewTransactions = true,
            AllowViewSupervisors = true,
            AllowAddTransactions = true,
            AllowAddSupervisors = true,
            AllowControlSupervised = true
        };

        public Login()
        {
            InitializeComponent();
        }




        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentPermissions = new SupervisorInfo
            {
                AllowViewCharts = true,
                AllowViewTransactions = true,
                AllowViewSupervisors = true,
                AllowAddTransactions = true,
                AllowAddSupervisors = true,
                AllowControlSupervised = true
            };

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            byte[] passwordHash = PasswordManager.HashPassword(password);
            var userId = await Database.CheckUserCredentials(username, passwordHash);

            if (userId.HasValue)
            {
                CurrentUserId = userId.Value;
                Current_money = await Database.GetUserCurrentMoney();
                await LoadTransactions(userId.Value);
                NavigationService.Navigate(new Transactions());
            }
            else
            {
                MessageBox.Show(Application.Current.FindResource("InvalidUsernamePassword") as string);
            }
        }



        private async Task LoadTransactions(int userId)
        {
            transactions = await Database.LoadTransactionsFromDatabase(userId);
        }

        private void CreateAccountButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateAccount());
        }



        private void UsaFlag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeLanguage("en-US");
        }

        private void SlovakiaFlag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeLanguage("sk");
        }

        private void ChangeLanguage(string cultureCode)
        {
            LanguageManager.LoadResourceDictionary(cultureCode);

            Properties.Settings.Default.PreferredLanguage = cultureCode;
            Properties.Settings.Default.Save();
        }
    }
}
