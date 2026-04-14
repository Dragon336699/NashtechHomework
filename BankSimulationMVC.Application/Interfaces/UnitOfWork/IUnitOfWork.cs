using BankSimulationMVC.Application.Interfaces.Repositories;

namespace BankSimulationMVC.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        ITransactionRepository Transactions { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
