namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class DepositVM
    {
        public required string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
