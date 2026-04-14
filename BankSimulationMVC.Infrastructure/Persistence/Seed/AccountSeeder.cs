using BankSimulationMVC.Application.Interfaces.Seeds;
using BankSimulationMVC.Data;
using BankSimulationMVC.Domain.Entities;

namespace BankSimulationMVC.Infrastructure.Persistence.Seed
{
    public class AccountSeeder : ISeeder
    {
        private readonly BankDbContext _context;
        public AccountSeeder(BankDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Accounts.Any()) return;

            List<Account> accounts = new List<Account>
            {
                new Account
                {
                    AccountNumber = "0866616877",
                    OwnerName = "Nguyễn Vũ B",
                },
                new Account
                {
                    AccountNumber = "0973585899",
                    OwnerName = "Lê T",
                },
                new Account
                {
                    AccountNumber = "0973856738",
                    OwnerName = "Nguyễn Văn A",
                },
                new Account
                {
                    AccountNumber = "0987654333",
                    OwnerName = "Nguyễn Khánh",
                }
            };
            _context.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync();
        }
    }
}
