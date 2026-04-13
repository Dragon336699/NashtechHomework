using BankSimulationMVC.Enum;
using BankSimulationMVC.Models;

namespace BankSimulationMVC.Interfaces
{
    public interface ITransactionService
    {
        IQueryable<Transaction> Query();
        Task<List<Transaction>> GetPageTransaction(int page, int pageSize);
        List<Transaction> GetTransactionById(string accountNumber);

    }
}
