using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Interfaces.Repositories;

namespace BankSimulationMVC.Application.Interfaces.Repositories
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetPageTransaction(int page, int pageSize);
        IQueryable<Transaction> Query();
    }
}
