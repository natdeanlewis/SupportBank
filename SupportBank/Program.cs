using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;



namespace SupportBank
{
    internal class Program
    {
        
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            var transactions = new List<Transaction>();
            var balances = new Dictionary<string, decimal>();
            using (var reader =
                new StreamReader(@"C:\Work\Training\SupportBank\SupportBank-master\DodgyTransactions2015.csv"))
            {
                var headerLine = reader.ReadLine();
                string line;
                int lineCount = 2;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var values = line.Split(',');
                        var newTransaction = new Transaction();
                        newTransaction.date = values[0];
                        newTransaction.to = values[1];
                        newTransaction.from = values[2];
                        newTransaction.narrative = values[3];
                        newTransaction.amount = Convert.ToDecimal(values[4]);

                        transactions.Add(newTransaction);

                        if (balances.ContainsKey(newTransaction.to))
                            balances[newTransaction.to] += newTransaction.amount;
                        else
                            balances[newTransaction.to] = newTransaction.amount;
                        if (balances.ContainsKey(newTransaction.from))
                            balances[newTransaction.from] -= newTransaction.amount;
                        else
                            balances[newTransaction.from] = -1 * newTransaction.amount;
                    }
                    catch
                    {
                        logger.Debug("Error storing line " + lineCount + " as a transaction");
                    }

                    lineCount++;

                }
            }

            var command = Console.ReadLine().ToLower();
            if (command == "list all")
            {
                Console.WriteLine("Here are the current net balances:");
                foreach (var item in balances)
                {
                    var netPhrase = "";
                    decimal netAmount = 0;
                    switch (item.Value)
                    {
                        case < 0:
                            netPhrase = "owes";
                            netAmount = item.Value * -1;
                            break;
                        default:
                            netPhrase = "is owed";
                            netAmount = item.Value;
                            break;
                    }

                    Console.WriteLine(string.Join(" ", item.Key, netPhrase, "£" + netAmount));
                }
            }
            else if (command.Substring(0, 5) == "list ")
            {
                var account = command.Substring(5);
                var myTI = new CultureInfo("en-UK", false).TextInfo;
                Console.WriteLine("Here are all the transactions associated with " + myTI.ToTitleCase(account) + ":");
                foreach (var transaction in transactions)
                    if ((transaction.from.ToLower() == account) | (transaction.to.ToLower() == account))
                        Console.WriteLine(string.Join(" ", "On", transaction.date + ",", transaction.from, "paid",
                            transaction.to, "£" + transaction.amount, "for", transaction.narrative.ToLower()));
            }
        }
    }
}