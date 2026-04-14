using BankSimulationMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankSimulationMVC.Data
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);
        }
    }
}
