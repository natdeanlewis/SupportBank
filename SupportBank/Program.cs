using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var reader =
                new StreamReader(@"C:\Work\Training\SupportBank\SupportBank-master\Transactions2014.csv"))
            {
                string headerLine = reader.ReadLine();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    Transaction newTransaction = new Transaction();
                    newTransaction.date = values[0];
                    newTransaction.to = values[1];
                    newTransaction.from = values[2];
                    newTransaction.narrative = values[3];
                    newTransaction.amount = Convert.ToDecimal(values[4]);
                    
                    transactions.Add(newTransaction);
                }

                foreach (var transaction in transactions)
                {
                    Console.WriteLine(transaction.date);
                    Console.WriteLine(transaction.from);
                    Console.WriteLine(transaction.to);
                    Console.WriteLine(transaction.narrative);
                    Console.WriteLine(transaction.amount);
                }
            }
        }
    }

    class Transaction
    {
        public string date;
        public string from;
        public string to;
        public string narrative;
        public decimal amount;
    }
}