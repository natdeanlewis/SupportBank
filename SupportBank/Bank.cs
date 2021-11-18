using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    public class Bank
    {
        public List<Transaction> transactions = new List<Transaction>();
        public Dictionary<string, decimal> balances = new Dictionary<string, decimal>();

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void readAndOrganise()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            
            logger.Info("Starting to read csv");
            using (var reader =
                new StreamReader(@"C:\Work\Training\SupportBank\SupportBank-master\Transactions2014.csv"))
            {
                var headerLine = reader.ReadLine();
                string line;
                int lineCount = 2;
                logger.Info("Organising data into transactions and user balances");

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
                        logger.Error("Error storing line " + lineCount + " as a transaction");
                        throw;
                    }

                    lineCount++;

                }
            }
        }

        public void runCommand()
        {
            Console.WriteLine("Type 'list all' to see current user balances, or 'list <name>' to see all of a  user's transactions:");
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