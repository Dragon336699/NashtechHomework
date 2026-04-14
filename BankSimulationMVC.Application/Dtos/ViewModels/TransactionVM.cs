using BankSimulationMVC.Domain.Entities;

namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class TransactionVM
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public string AccountNumber { get; set; }
    }
}
