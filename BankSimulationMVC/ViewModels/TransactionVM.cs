using BankSimulationMVC.Models;

namespace BankSimulationMVC.ViewModels
{
    public class TransactionVM
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public string AccountNumber { get; set; }
    }
}
