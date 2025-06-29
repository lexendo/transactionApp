using System.Collections.Generic;
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
        }

        private async void LoadAndDisplayTransactions()
        {
            //List<Transaction> transactions = TransactionManager.LoadTransactionsFromFiles();

            TransactionsListBox.ItemsSource = Login.transactions;
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