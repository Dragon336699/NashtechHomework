namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class WithDrawVM
    {
        public required string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
