using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum TransactionType
{
    Entertainment = 1,
    Healthcare = 2,
    Insurance = 3,
    Groceries = 4,
    Gift = 5,
    Tax = 6,
    Rent = 7,
    Shopping = 8,
    Other = 0
}


namespace NotSoEpicApp
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsIncome { get; set; }
        public TransactionType Type { get; set; } 
    }
}
