using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Data;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankSimulationMVC.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly BankDbContext _context;
        public TransactionRepository(BankDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetPageTransaction(int page, int pageSize)
        {
            return await _context.Transactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public IQueryable<Transaction> Query()
        {
            return _context.Transactions.AsQueryable();
        }
    }
}
