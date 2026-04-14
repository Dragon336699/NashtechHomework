using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Application.Dtos.ViewModels;
using BankSimulationMVC.Application.Dtos.Responses;

namespace BankSimulationMVC.Interfaces.Services
{
    public interface IAccountService
    {
        IEnumerable<AccountVM> GetAllAccounts();
        Task<ServiceResult> CreateAccount(Account account);
        Task<ServiceResult> Deposit(DepositVM depositViewModel);
        Task<ServiceResult> Withdraw(WithDrawVM withdrawViewModel);
        Task<AccountVM?> GetAccountDetails(string accountNumber);
        Task<ServiceResult> Transfer(TransferVM transferViewModel);
        Task<ServiceResult> ToggleAccountStatus(string accountNumber);
        Task ProcessMonthlyInterest();
    }
}
