using BankSimulationMVC.Models;
using BankSimulationMVC.ViewModels;

namespace BankSimulationMVC.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<AccountDto> GetAllAccounts();
        Task<ServiceResult> CreateAccount(Account account);
        Task<ServiceResult> Deposit(DepositVM depositViewModel);
        Task<ServiceResult> Withdraw(WithDrawVM withdrawViewModel);
        Task<AccountDto?> GetAccountDetails(string accountNumber);
        Task<ServiceResult> Transfer(TransferVM transferViewModel);
        Task<ServiceResult> ToggleAccountStatus(string accountNumber);
        Task ProcessMonthlyInterest();
    }
}
