using System.Collections.Generic;
using System.Linq;

namespace pruebadedesempeño.Models
{
    public class Associate
    {
        public string DocumentId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Associate(string documentId, string fullName, string phoneNumber, string address)
        {
            DocumentId = documentId;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public decimal GetBalance()
        {
            decimal totalDeposits = Transactions
                .Where(t => t.Type == TransactionType.Deposit)
                .Sum(t => t.Amount);

            decimal totalWithdrawals = Transactions
                .Where(t => t.Type == TransactionType.Withdrawal)
                .Sum(t => t.TotalDeduction);

            return totalDeposits - totalWithdrawals;
        }
    }
}
