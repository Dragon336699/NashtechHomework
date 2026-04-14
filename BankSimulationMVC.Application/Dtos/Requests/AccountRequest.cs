namespace BankSimulationMVC.Application.Dtos.Requests
{
    public class AccountRequest
    {
        public required string AccountNumber { get; set; }
        public required string OwnerName { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
