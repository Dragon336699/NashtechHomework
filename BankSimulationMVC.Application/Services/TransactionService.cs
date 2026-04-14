using BankSimulationMVC.Application.Interfaces.UnitOfWork;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Interfaces.Services;

namespace BankSimulationMVC.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Transaction>> GetPageTransaction(int page, int pageSize)
        {
            return await _unitOfWork.Transactions.GetPageTransaction(page, pageSize);
        }

        public IEnumerable<Transaction> GetTransactionsById(string accountNumber)
        {
            return _unitOfWork.Transactions.Find(t => t.AccountNumber == accountNumber);
        }

        public IQueryable<Transaction> Query()
        {
            return _unitOfWork.Transactions.Query();
        }
    }
}
