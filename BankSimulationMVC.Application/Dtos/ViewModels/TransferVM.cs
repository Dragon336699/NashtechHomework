namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class TransferVM
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
