using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Application.Dtos.Requests;
using BankSimulationMVC.Application.Dtos.ViewModels;

namespace BankSimulationMVC.Mapper
{
    public static class AccountMapper
    {
        public static Account ToDomain(this AccountRequest dto)
        {
            var account = new Account
            {
                AccountNumber = dto.AccountNumber,
                OwnerName = dto.OwnerName,
                CreatedAt = DateTime.Now,
                Status = Enum.AccountStatus.Active
            };

            account.UpdateBalance(dto.InitialBalance);

            return account;
        }

        public static AccountVM ToDto(this Account account)
        {
            return new AccountVM
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
