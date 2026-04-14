using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Data;
using BankSimulationMVC.Domain.Entities;

namespace BankSimulationMVC.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(BankDbContext context) : base(context)
        {
        }
    }
}
