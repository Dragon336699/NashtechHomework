using BankAccountSimulation.Enum;

namespace BankAccountSimulation
{
    public class Transaction
    {
        public required string AccountNumber { get; set; }
        public int TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        public override string ToString()
        {
            return $""""
                [Account Number: {AccountNumber}] [Date: {Date}] [Type: {Type}] [Amount: {Amount}] {(Description == null ? "" : $"[Description: {Description}]")}
                ----------------------------------------------------------------------------
                """";
        }
    }
}
