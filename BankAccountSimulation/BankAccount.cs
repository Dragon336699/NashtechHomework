using BankAccountSimulation.Enum;
using System.Diagnostics.CodeAnalysis;

namespace BankAccountSimulation
{
    public class BankAccount
    {
        public decimal DailyWithdrawLimit = 50000000;
        public required string AccountNumber { get; set; }
        public required string OwnerName { get; set; }
        public decimal Balance { get; private set; }
        public BankAccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal WithdrawnToday { get; set; };
        public DateTime LastWithdrawDate { get; set; }
        [SetsRequiredMembers]
        public BankAccount(string accountNumber, string ownerName, decimal balance)
        {
            AccountNumber = accountNumber;
            OwnerName = ownerName;
            Balance = balance;
            Status = BankAccountStatus.Active;
            CreatedAt = DateTime.Now;
            WithdrawnToday = 0;
        }
        public override string ToString()
        {
            return $""""
                    [Account number: {AccountNumber}] [Owner Name: {OwnerName}] [Balance: {Balance}] [Status: {Status}] [Created Date: {CreatedAt}]
                    ------------------------------------------------------------------------------------------------------------------------------
                """";
        }

        public void DepositMoney(decimal amount)
        {
            if (amount <= 0 )
                throw new ArgumentOutOfRangeException("Amount must be greater than 0");
            if (Status == BankAccountStatus.Frozen)
                throw new InvalidOperationException($"Account {AccountNumber} is frozen. Please try again!");
            Balance += amount;
        }

        public void WithDrawMoney(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Amount must be greater than 0");
            if (Status == BankAccountStatus.Frozen)
                throw new InvalidOperationException($"Account {AccountNumber} is frozen. Please try again!");
            if (Balance < amount)
            {
                throw new InvalidOperationException("Your balance is not enough. Please try again!");
            }
            if (Balance - amount < 100)
            {
                throw new InvalidOperationException("Your minimal balance is 100. Please try again!");
            }

            if (WithdrawnToday + amount > DailyWithdrawLimit)
                throw new InvalidOperationException("Daily withdraw limit exceeded");
            Balance -= amount;

            WithdrawnToday += amount;
            LastWithdrawDate = DateTime.Now;
        }

        private void ResetWithdrawDaily ()
        {
            if (LastWithdrawDate.Date < DateTime.Now.Date)
            {
                WithdrawnToday = 0;
                LastWithdrawDate = DateTime.Now;
            }
        }
    }
}
