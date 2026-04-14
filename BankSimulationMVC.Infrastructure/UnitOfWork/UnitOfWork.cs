using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Application.Interfaces.UnitOfWork;
using BankSimulationMVC.Data;

namespace BankSimulationMVC.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankDbContext _context;
        public IAccountRepository Accounts { get; private set; }
        public ITransactionRepository Transactions { get; private set; }


        public UnitOfWork(
            BankDbContext context,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository
            )
        {
            _context = context;
            Accounts = accountRepository;
            Transactions = transactionRepository;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
