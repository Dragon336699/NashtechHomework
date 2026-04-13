using BankSimulationMVC.Models;
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
            builder.Entity<Account>()
                .HasKey(a => a.AccountNumber);
            builder.Entity<Account>()
                .Property(a => a.AccountNumber)
                .ValueGeneratedNever();
        }
    }
}
