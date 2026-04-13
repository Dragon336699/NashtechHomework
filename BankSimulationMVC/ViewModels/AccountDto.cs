using BankSimulationMVC.Enum;
using System.ComponentModel.DataAnnotations;

namespace BankSimulationMVC.ViewModels
{
    public class AccountDto
    {
        [Required(ErrorMessage = "Account number is required")]
        public required string AccountNumber { get; set; }
        [Required(ErrorMessage = "Owner is required")]
        public required string OwnerName { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Balance must be greater than 0")]
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
