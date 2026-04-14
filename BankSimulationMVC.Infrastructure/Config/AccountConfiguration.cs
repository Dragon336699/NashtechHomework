using BankSimulationMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BankSimulationMVC.Infrastructure.Config
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.AccountNumber);

            builder.Property(a => a.OwnerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired();
            builder
                .Property(x => x.WithdrawLimit)
                .HasPrecision(18, 2)
                .HasDefaultValue(500000000);
            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.LastAddInterestMonthlyDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.InterestMonthly)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.HasIndex(a => a.AccountNumber)
                .IsUnique();
        }
    }
}
