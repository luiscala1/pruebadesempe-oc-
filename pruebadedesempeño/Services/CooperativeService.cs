
using pruebadedesempeño.Models;

namespace pruebadedesempeño.Services
{
    public class CooperativeService
    {
        private readonly List<Associate> _associates = new List<Associate>();

        public bool RegisterAssociate(string documentId, string fullName, string phone, string address, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(documentId) || string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "El documento y el nombre no pueden estar vacíos.";
                return false;
            }

            bool alreadyExists = false;
            foreach (var a in _associates)
            {
                if (a.DocumentId.Equals(documentId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                errorMessage = "Ya existe un asociado registrado con ese número de documento.";
                return false;
            }

            var newAssociate = new Associate(documentId.Trim(), fullName.Trim(), phone?.Trim(), address?.Trim());
            _associates.Add(newAssociate);
            errorMessage = string.Empty;
            return true;
        }

        public List<Associate> GetAllAssociates()
        {
            return _associates;
        }

        public Associate FindByDocument(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return null;
            }

            Associate found = null;
            foreach (var a in _associates)
            {
                if (a.DocumentId.Equals(documentId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    found = a;
                    break;
                }
            }
            return found;
        }

        public List<Associate> FindByName(string nameQuery)
        {
            if (string.IsNullOrWhiteSpace(nameQuery))
            {
                return new List<Associate>();
            }

            string query = nameQuery.Trim();
            var results = new List<Associate>();
            foreach (var a in _associates)
            {
                if (a.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(a);
                }
            }
            return results;
        }

        public bool UpdateAssociate(string documentId, string newFullName, string newPhone, string newAddress, out string errorMessage)
        {
            var associate = FindByDocument(documentId);
            if (associate == null)
            {
                errorMessage = "Asociado no encontrado.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(newFullName))
            {
                associate.FullName = newFullName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                associate.PhoneNumber = newPhone.Trim();
            }
            if (!string.IsNullOrWhiteSpace(newAddress))
            {
                associate.Address = newAddress.Trim();
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool DeleteAssociate(string documentId, out string errorMessage)
        {
            var associate = FindByDocument(documentId);
            if (associate == null)
            {
                errorMessage = "Asociado no encontrado.";
                return false;
            }

            if (associate.Transactions.Count > 0 || associate.GetBalance() > 0)
            {
                errorMessage = "No se puede eliminar un asociado con saldo o con movimientos registrados en el historial.";
                return false;
            }

            _associates.Remove(associate);
            errorMessage = string.Empty;
            return true;
        }

        public bool ProcessDeposit(string documentId, decimal amount, out string errorMessage)
        {
            if (amount <= 0)
            {
                errorMessage = "El monto a consignar debe ser mayor a cero.";
                return false;
            }

            var associate = FindByDocument(documentId);
            if (associate == null)
            {
                errorMessage = "Asociado no encontrado.";
                return false;
            }

            associate.Transactions.Add(new Transaction(TransactionType.Deposit, amount));
            errorMessage = string.Empty;
            return true;
        }

        public bool ProcessWithdrawal(string documentId, decimal amount, out string errorMessage)
        {
            if (amount <= 0)
            {
                errorMessage = "El monto a retirar debe ser mayor a cero.";
                return false;
            }

            var associate = FindByDocument(documentId);
            if (associate == null)
            {
                errorMessage = "Asociado no encontrado.";
                return false;
            }

            decimal fee = 0;
            if (amount > 1000000)
            {
                fee = 8000;
            }

            decimal totalRequired = amount + fee;

            if (associate.GetBalance() < totalRequired)
            {
                errorMessage = $"Saldo insuficiente. Requiere ${totalRequired:N0} (Monto: ${amount:N0} + Comisión: ${fee:N0}), pero el saldo actual es ${associate.GetBalance():N0}.";
                return false;
            }

            associate.Transactions.Add(new Transaction(TransactionType.Withdrawal, amount, fee));
            errorMessage = string.Empty;
            return true;
        }

        public (decimal TotalBalance, int TotalAssociates, decimal AverageBalance) GetGeneralBalanceReport()
        {
            int count = _associates.Count;
            decimal total = 0;

            foreach (var a in _associates)
            {
                total += a.GetBalance();
            }

            decimal average = count > 0 ? total / count : 0;
            return (total, count, average);
        }

        public List<Associate> GetTop5Associates()
        {
            return _associates
                .OrderByDescending(a => a.GetBalance())
                .Take(5)
                .ToList();
        }

        public List<Associate> GetDormantAssociates()
        {
            var dormant = new List<Associate>();
            foreach (var a in _associates)
            {
                if (a.Transactions.Count == 0)
                {
                    dormant.Add(a);
                }
            }
            return dormant;
        }

        public (decimal TotalDeposited, int DepositCount, decimal TotalWithdrawn, int WithdrawalCount, decimal NetDifference) GetPeriodReport(DateTime startDate, DateTime endDate)
        {
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);

            var allTransactions = new List<Transaction>();
            foreach (var a in _associates)
            {
                foreach (var t in a.Transactions)
                {
                    if (t.TransactionDate >= startDate.Date && t.TransactionDate <= endOfDay)
                    {
                        allTransactions.Add(t);
                    }
                }
            }

            decimal totalIn = 0;
            int depositCount = 0;
            decimal totalOut = 0;
            int withdrawalCount = 0;

            foreach (var t in allTransactions)
            {
                if (t.Type == TransactionType.Deposit)
                {
                    totalIn += t.Amount;
                    depositCount++;
                }
                else if (t.Type == TransactionType.Withdrawal)
                {
                    totalOut += t.Amount;
                    withdrawalCount++;
                }
            }

            decimal net = totalIn - totalOut;
            return (totalIn, depositCount, totalOut, withdrawalCount, net);
        }

        public List<(Transaction Movement, string AssociateName)> GetTop10Transactions()
        {
            var all = new List<(Transaction Movement, string AssociateName)>();

            foreach (var a in _associates)
            {
                foreach (var t in a.Transactions)
                {
                    all.Add((t, a.FullName));
                }
            }

            return all
                .OrderByDescending(x => x.Movement.Amount)
                .Take(10)
                .ToList();
        }

        public List<(string Name, int MovementCount, decimal TotalDeposited, decimal TotalWithdrawn, decimal Balance)> GetAssociateActivityReport()
        {
            var result = new List<(string Name, int MovementCount, decimal TotalDeposited, decimal TotalWithdrawn, decimal Balance)>();

            foreach (var a in _associates)
            {
                decimal deposited = 0;
                decimal withdrawn = 0;

                foreach (var t in a.Transactions)
                {
                    if (t.Type == TransactionType.Deposit)
                    {
                        deposited += t.Amount;
                    }
                    else if (t.Type == TransactionType.Withdrawal)
                    {
                        withdrawn += t.Amount;
                    }
                }

                result.Add((a.FullName, a.Transactions.Count, deposited, withdrawn, a.GetBalance()));
            }

            return result.OrderByDescending(x => x.MovementCount).ToList();
        }
    }
}
