using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NotSoEpicApp
{
    public static class TransactionManager
    {
        public static List<Transaction> LoadTransactionsFromFiles()
        {
            string directoryPath = Path.Combine(Environment.CurrentDirectory, "Transactions");
            List<Transaction> transactions = new List<Transaction>();

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.txt");

                foreach (var file in files)
                {
                    string[] lines = File.ReadAllLines(file);

                    decimal amount = decimal.Parse(lines[1].Replace("Amount: ", ""));

                    Transaction transaction = new Transaction
                    {
                        Title = lines[0].Replace("Title: ", ""),
                        Amount = amount,
                        IsIncome = amount >= 0,
                        Description = lines.Length > 3 ? lines[lines.Length - 2].Replace("Description: ", "") : string.Empty,
                        Date = DateTime.ParseExact(lines[lines.Length - 1].Replace("Date: ", ""), "M/d/yyyy", CultureInfo.InvariantCulture)
                    };

                    transactions.Add(transaction);
                }

                transactions = transactions.OrderByDescending(t => t.Date).ToList();
            }

            return transactions;
        }

        public static (decimal TotalIncome, decimal TotalExpenses) CalculateTotalsForPeriod(DateTime startDate, DateTime endDate)
        {
            List<Transaction> transactions = LoadTransactionsFromFiles();

            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Date >= startDate && transaction.Date <= endDate)
                {
                    if (transaction.IsIncome)
                    {
                        totalIncome += transaction.Amount;
                    }
                    else
                    {
                        totalExpenses += transaction.Amount;
                    }
                }
            }

            return (totalIncome, totalExpenses);
        }

        public static void SaveTransactionToFile(Transaction transaction)
        {
            string directoryPath = Path.Combine(Environment.CurrentDirectory, "Transactions");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = $"{transaction.Title}.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            int fileNumber = 1;
            while (File.Exists(filePath))
            {
                fileName = $"{transaction.Title}_{fileNumber}.txt";
                filePath = Path.Combine(directoryPath, fileName);
                fileNumber++;
            }

            string[] lines = {
                $"Title: {transaction.Title}",
                $"Amount: {transaction.Amount}",
                $"IsIncome: {transaction.IsIncome}",
                $"Description: {transaction.Description}",
                $"Date: {transaction.Date.ToShortDateString()}"
            };

            File.WriteAllLines(filePath, lines);
        }
    }
}
