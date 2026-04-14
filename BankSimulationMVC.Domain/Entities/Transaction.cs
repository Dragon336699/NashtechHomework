using BankSimulationMVC.Enum;

namespace BankSimulationMVC.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public required string AccountNumber { get; set; }
    }
}
