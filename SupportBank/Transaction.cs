namespace SupportBank
{
    public class Transaction
    {
        public decimal amount;
        public string date;
        public string from;
        public string narrative;
        public string to;

        public override string ToString()
        {
            return string.Join(" ", date, from, to, narrative, amount.ToString());
        }
    }
}