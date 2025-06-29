using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace NotSoEpicApp
{
    /// <summary>
    /// Interaction logic for ManageAccount.xaml
    /// </summary>
    public partial class ManageAccount : Page
    {
        public ManageAccount()
        {
            InitializeComponent();
            LoadUsers();
            SetAsSupervisorCheckBox.Checked += SetAsSupervisorCheckBox_Checked;
            SetAsSupervisorCheckBox.Unchecked += SetAsSupervisorCheckBox_Unchecked;

            AllowViewTransactionsCheckBox.Checked += ViewPermissionChanged;
            AllowViewTransactionsCheckBox.Unchecked += ViewPermissionChanged;

            AllowViewSupervisorsCheckBox.Checked += ViewPermissionChanged;
            AllowViewSupervisorsCheckBox.Unchecked += ViewPermissionChanged;
        }

        private async void LoadUsers()
        {
            try
            {
                List<string> users = await Database.GetAllOtherUsers();
                UserDropdown.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading users: {ex.Message}");
            }
        }

        private async void UserDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserDropdown.SelectedItem != null)
            {
                string selectedUser = UserDropdown.SelectedItem.ToString();

                var supervisorInfo = await Database.GetSupervisorInfo(selectedUser);

                if (supervisorInfo != null)
                {
                    SetAsSupervisorCheckBox.IsChecked = true;
                    AllowViewTransactionsCheckBox.IsChecked = supervisorInfo.AllowViewTransactions;
                    AllowViewSupervisorsCheckBox.IsChecked = supervisorInfo.AllowViewSupervisors;
                    AllowViewChartsCheckBox.IsChecked = supervisorInfo.AllowViewCharts;
                    AllowAddTransactionsCheckBox.IsChecked = supervisorInfo.AllowAddTransactions;
                    AllowAddSupervisorsCheckBox.IsChecked = supervisorInfo.AllowAddSupervisors;
                    EnableAccessCheckBoxes(true);
                }
                else
                {
                    SetAsSupervisorCheckBox.IsChecked = false;
                    EnableAccessCheckBoxes(false);
                    ClearAccessCheckBoxes();
                }
            }
            EnableAccessCheckBoxes(SetAsSupervisorCheckBox.IsChecked == true);
        }

        private void SetAsSupervisorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EnableAccessCheckBoxes(true);
        }

        private void SetAsSupervisorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableAccessCheckBoxes(false);
            ClearAccessCheckBoxes();
        }

        private void EnableAccessCheckBoxes(bool isEnabled)
        {
            AllowViewTransactionsCheckBox.IsEnabled = isEnabled;
            AllowViewSupervisorsCheckBox.IsEnabled = isEnabled;
            AllowViewChartsCheckBox.IsEnabled = isEnabled;

            if (isEnabled && AllowViewTransactionsCheckBox.IsChecked == true)
            {
                AllowAddTransactionsCheckBox.IsEnabled = true;
            }
            else
            {
                AllowAddTransactionsCheckBox.IsEnabled = false;
                AllowAddTransactionsCheckBox.IsChecked = false;
            }

            if (isEnabled && AllowViewSupervisorsCheckBox.IsChecked == true)
            {
                AllowAddSupervisorsCheckBox.IsEnabled = true;
            }
            else
            {
                AllowAddSupervisorsCheckBox.IsEnabled = false;
                AllowAddSupervisorsCheckBox.IsChecked = false;
            }
        }

        private void ClearAccessCheckBoxes()
        {
            AllowViewTransactionsCheckBox.IsChecked = false;
            AllowViewSupervisorsCheckBox.IsChecked = false;
            AllowViewChartsCheckBox.IsChecked = false;
            AllowAddTransactionsCheckBox.IsChecked = false;
            AllowAddSupervisorsCheckBox.IsChecked = false;
        }

        private void ViewPermissionChanged(object sender, RoutedEventArgs e)
        {
            if (SetAsSupervisorCheckBox.IsChecked == true)
            {
                EnableAccessCheckBoxes(true);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDropdown.SelectedItem != null)
            {
                string selectedUser = UserDropdown.SelectedItem.ToString();
                bool isSupervisor = SetAsSupervisorCheckBox.IsChecked ?? false;

                if (isSupervisor)
                {
                    bool allowViewTransactions = AllowViewTransactionsCheckBox.IsChecked ?? false;
                    bool allowViewSupervisors = AllowViewSupervisorsCheckBox.IsChecked ?? false;
                    bool allowViewCharts = AllowViewChartsCheckBox.IsChecked ?? false;
                    bool allowAddTransactions = AllowAddTransactionsCheckBox.IsChecked ?? false;
                    bool allowAddSupervisors = AllowAddSupervisorsCheckBox.IsChecked ?? false;

                    var supervisorInfo = await Database.GetSupervisorInfo(selectedUser);

                    await Database.AddOrUpdateSupervisor(selectedUser, allowViewTransactions, allowViewSupervisors, allowViewCharts, allowAddTransactions, allowAddSupervisors);
                    if (supervisorInfo != null)
                    {
                        MessageBox.Show("Supervisor updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Supervisor added successfully.");
                    }
                }
                else
                {
                    await Database.DeleteSupervisor(selectedUser);
                    MessageBox.Show("Supervisor removed.");
                }
            }
            else
            {
                MessageBox.Show("Please select a user.");
            }
        }
        private void TransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Transactions());
        }
    }
}
