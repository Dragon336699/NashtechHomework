using BankSimulationMVC.Data;
using BankSimulationMVC.Enum;
using BankSimulationMVC.Interfaces;
using BankSimulationMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSimulationMVC.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BankDbContext _context;
        public TransactionService(BankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetPageTransaction(int page, int pageSize)
        {
            return await _context.Transactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public List<Transaction> GetTransactionById(string accountNumber)
        {
            return _context.Transactions.Where(t => t.AccountNumber == accountNumber).ToList();
        }

        public IQueryable<Transaction> Query ()
        {
            return _context.Transactions.AsQueryable();
        }
    }
}
