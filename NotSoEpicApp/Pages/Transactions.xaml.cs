using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NotSoEpicApp
{
    public partial class Transactions : Page
    {
        public Transactions()
        {
            InitializeComponent();
            LoadAndDisplayTransactions();
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            var p = Login.CurrentPermissions;

            ChartsButton.IsEnabled = p.AllowViewCharts;

            if (!p.AllowViewCharts)
            {
                SetDisabledButtonTooltip(ChartsButton, Application.Current.FindResource("ChartsNotAllowed"));
            }

            if (!p.AllowViewTransactions)
            {
                TransactionsListSection.Visibility = Visibility.Collapsed;
                TransactionsTitleHeader.Visibility = Visibility.Collapsed;
                TransactionsTypeHeader.Visibility = Visibility.Collapsed;
                TransactionsAmountHeader.Visibility = Visibility.Collapsed;
                TransactionsDateHeader.Visibility = Visibility.Collapsed;
                DescriptionTextBlock.Visibility = Visibility.Hidden;

                TransactionsNoAccessPanel.Visibility = Visibility.Visible;
            }
            else
            {
                TransactionsListSection.Visibility = Visibility.Visible;
                DescriptionTextBlock.Visibility = Visibility.Visible;

                TransactionsNoAccessPanel.Visibility = Visibility.Collapsed;
            }

            AddTransactionButton.IsEnabled = p.AllowAddTransactions;
            if (!p.AllowAddTransactions)
            {
                SetDisabledButtonTooltip(AddTransactionButton, Application.Current.FindResource("AddTransactionNotAllowed"));
            }

        }

        private void SetDisabledButtonTooltip(Button button, object tooltipContent)
        {
            var tooltip = new ToolTip
            {
                Content = tooltipContent,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse
            };
            ToolTipService.SetToolTip(button, tooltip);
            ToolTipService.SetShowOnDisabled(button, true);
            ToolTipService.SetInitialShowDelay(button, 0);
        }


        private async void LoadAndDisplayTransactions()
        {

            TransactionsListBox.ItemsSource = Login.transactions.OrderByDescending(t => t.Date).ToList();
        }

        private void Charts_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Charts());
        }

        private void AddTransaction_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new NewTransactionPage());
        }

        private void MyAccount_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageAccount());
        }

        private void TransactionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Transaction selectedTransaction = (Transaction)TransactionsListBox.SelectedItem;
            if (selectedTransaction != null)
            {
                DescriptionTextBlock.Text = selectedTransaction.Description;
            }
            else
            {
                DescriptionTextBlock.Text = "Select a transaction to see the description.";
            }
        }
    }
}