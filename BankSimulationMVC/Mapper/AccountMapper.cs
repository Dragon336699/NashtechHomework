using BankSimulationMVC.Models;
using BankSimulationMVC.ViewModels;

namespace BankSimulationMVC.Mapper
{
    public static class AccountMapper
    {
        public static Account ToDomain(this AccountDto dto)
        {
            var account = new Account
            {
                AccountNumber = dto.AccountNumber,
                OwnerName = dto.OwnerName,
                CreatedAt = DateTime.Now,
                Status = Enum.AccountStatus.Active
            };

            account.UpdateBalance(dto.Balance);

            return account;
        }

        public static AccountDto ToDto(this Account account)
        {
            return new AccountDto
            {
                AccountNumber = account.AccountNumber,
                OwnerName = account.OwnerName,
                Balance = account.Balance,
                CreatedAt = DateTime.Now,
                Status = account.Status
            };
        }
    }
}
