using System;
using System.IO;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader =
                new StreamReader(@"C:\Work\Training\SupportBank\SupportBank-master\Transactions2014.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}