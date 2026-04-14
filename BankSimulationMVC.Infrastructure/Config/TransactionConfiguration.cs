using BankSimulationMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSimulationMVC.Infrastructure.Config
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.TransactionId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Type)
                .IsRequired();

            builder.Property(a => a.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.AccountNumber)
            .IsRequired();
        }
    }
}
