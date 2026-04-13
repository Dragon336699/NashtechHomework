using BankSimulationMVC.Enum;
using System.ComponentModel.DataAnnotations;

namespace BankSimulationMVC.Models
{
    public class Account
    {
        public required string AccountNumber { get; set; }
        public required string OwnerName { get; set; }
        public decimal Balance { get; private set; }
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal WithdrawLimit { get; set; } = 500000000;
        public DateTime LastWithdrawDate { get; set; }
        public DateTime LastAddInterestMonthlyDate { get; set; } = DateTime.Now;
        public decimal InterestMonthly { get; set; } = 0;

        public void UpdateBalance(decimal balance)
        {
            Balance = balance;
        }

        public void ResetWithdrawLimit()
        {
            WithdrawLimit = 500000000;
        }
    }
}
