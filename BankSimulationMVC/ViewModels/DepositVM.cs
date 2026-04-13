using System.ComponentModel.DataAnnotations;

namespace BankSimulationMVC.ViewModels
{
    public class DepositVM
    {
        public required string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
