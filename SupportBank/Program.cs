
namespace SupportBank
{
    internal class Program
    {
        
        private static void Main(string[] args)
        {

            Bank newBank = new Bank();
            
            newBank.readAndOrganise();
            
            newBank.runCommand();
            
        }
    }
}