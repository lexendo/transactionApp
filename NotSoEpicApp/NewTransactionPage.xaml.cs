using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace NotSoEpicApp
{
    public partial class NewTransactionPage : Page
    {
        public NewTransactionPage()
        {
            InitializeComponent();
        }

        private async void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                string message = Application.Current.FindResource("TitleValidation") as string;
                MessageBox.Show(message);
                return;
            }

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                string message = Application.Current.FindResource("AmountValidation") as string;
                MessageBox.Show(message);
                return;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                string message = Application.Current.FindResource("DateValidation") as string;
                MessageBox.Show(message);
                return;
            }

            bool isIncome;
            if (IncomeRadioButton.IsChecked == true)
            {
                isIncome = true;
            }
            else
            {
                isIncome = false;
            }
            TransactionType selectedType;
            if (Enum.TryParse(TransactionTypeComboBox.Text, out selectedType))
            {
            }
            else
            {
                string message = Application.Current.FindResource("TransactionTypeValidation") as string;
                MessageBox.Show(message);
                return;
            }

            Transaction transaction = new Transaction
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                Amount = amount,
                Date = DatePicker.SelectedDate.Value,
                IsIncome = isIncome,
                Type = selectedType
            };


            bool success = await Database.SaveTransactionToDatabase(transaction);
            if(success == true)
            {
                Login.transactions.Add(transaction);

                if (transaction.IsIncome)
                {
                    Login.Current_money += transaction.Amount;
                }
                else
                {
                    Login.Current_money -= transaction.Amount;
                }

                bool updateSuccess = await Database.UpdateUserCurrentMoney(Login.Current_money.Value);


                string message = Application.Current.FindResource("TransactionSaveSuccess") as string;
                MessageBox.Show(message);
            }
            else
            {
                string message = Application.Current.FindResource("TransactionSaveFail") as string;
                MessageBox.Show(message);
            }



            TitleTextBox.Clear();
            AmountTextBox.Clear();
            DescriptionTextBox.Clear();
            DatePicker.SelectedDate = DateTime.Now;

        }

      
        private async void TransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Transactions());
        }

        private async void ChartsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Charts());
        }
    }
}
