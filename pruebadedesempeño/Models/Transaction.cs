using System;

namespace pruebadedesempeño.Models
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; } = 0;
        public decimal TotalDeduction => Amount + Fee;
        public DateTime TransactionDate { get; set; }

        public Transaction(TransactionType type, decimal amount, decimal fee = 0)
        {
            Type = type;
            Amount = amount;
            Fee = fee;
            TransactionDate = DateTime.Now;
        }
    }
}
