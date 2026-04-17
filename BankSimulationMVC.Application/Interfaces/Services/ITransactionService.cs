using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Enum;

namespace BankSimulationMVC.Interfaces.Services
{
    public interface ITransactionService
    {
        IQueryable<Transaction> Query();
        Task<IEnumerable<Transaction>> GetPageTransaction(int page, int pageSize);
        IEnumerable<Transaction> GetTransactionsByAccountNumber(string accountNumber);

    }
}
