using BankSimulationMVC.Enum;

namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class AccountVM
    {
        public required string AccountNumber { get; set; }
        public required string OwnerName { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
