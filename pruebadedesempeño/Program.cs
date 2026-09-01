using System;
using System.Globalization;
using System.Threading.Tasks;
using pruebadedesempeño.Models;
using pruebadedesempeño.Services;

namespace pruebadedesempeño
{
    internal class Program
    {
        private static readonly CooperativeService CooperativeService = new CooperativeService();
        private static readonly TrmService TrmService = new TrmService();

        static async Task Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("    COOPERATIVA FINANCIERA EL PROGRESO - CAJA     ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1.  Registrar un asociado nuevo");
                Console.WriteLine("2.  Listar todos los asociados");
                Console.WriteLine("3.  Buscar asociado por documento");
                Console.WriteLine("4.  Buscar asociado por nombre");
                Console.WriteLine("5.  Actualizar datos de un asociado");
                Console.WriteLine("6.  Eliminar un asociado");
                Console.WriteLine("7.  Consultar saldo en COP");
                Console.WriteLine("8.  Consultar saldo en USD (TRM oficial)");
                Console.WriteLine("9.  Registrar consignación");
                Console.WriteLine("10. Registrar retiro");
                Console.WriteLine("11. Ver movimientos de un asociado");
                Console.WriteLine("12. Informes de gerencia");
                Console.WriteLine("0.  Salir");
                Console.WriteLine("==================================================");
                Console.Write("Seleccione una opción: ");

                string option = Console.ReadLine();
                Console.Clear();

                if (option == "1")
                {
                    RegisterAssociateMenu();
                }
                else if (option == "2")
                {
                    ListAssociatesMenu();
                }
                else if (option == "3")
                {
                    SearchByDocumentMenu();
                }
                else if (option == "4")
                {
                    SearchByNameMenu();
                }
                else if (option == "5")
                {
                    UpdateAssociateMenu();
                }
                else if (option == "6")
                {
                    DeleteAssociateMenu();
                }
                else if (option == "7")
                {
                    CheckBalanceCopMenu();
                }
                else if (option == "8")
                {
                    await CheckBalanceUsdMenu();
                }
                else if (option == "9")
                {
                    ProcessDepositMenu();
                }
                else if (option == "10")
                {
                    ProcessWithdrawalMenu();
                }
                else if (option == "11")
                {
                    ViewTransactionsMenu();
                }
                else if (option == "12")
                {
                    await ReportsMenu();
                }
                else if (option == "0")
                {
                    exit = true;
                    Console.WriteLine("Cerrando sesión de caja. ¡Buen día!");
                }
                else
                {
                    Console.WriteLine("Opción no válida. Presione Enter para continuar.");
                }

                if (!exit)
                {
                    Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
                    Console.ReadKey();
                }
            }
        }

        private static void RegisterAssociateMenu()
        {
            Console.WriteLine("--- REGISTRAR NUEVO ASOCIADO ---");
            Console.Write("Número de Documento: ");
            string doc = Console.ReadLine();
            Console.Write("Nombre Completo: ");
            string name = Console.ReadLine();
            Console.Write("Teléfono: ");
            string phone = Console.ReadLine();
            Console.Write("Dirección: ");
            string address = Console.ReadLine();

            string error;
            bool success = CooperativeService.RegisterAssociate(doc, name, phone, address, out error);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✔ Asociado registrado exitosamente con saldo inicial de $0.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✖ Error: {error}");
            }
            Console.ResetColor();
        }

        private static void ListAssociatesMenu()
        {
            Console.WriteLine("--- LISTADO DE ASOCIADOS ---");
            var list = CooperativeService.GetAllAssociates();

            if (list.Count == 0)
            {
                Console.WriteLine("No hay asociados registrados.");
                return;
            }

            Console.WriteLine($"{"Documento",-15} | {"Nombre Completo",-25} | {"Teléfono",-15} | {"Saldo Actual",15}");
            Console.WriteLine(new string('-', 78));

            foreach (var a in list)
            {
                Console.WriteLine($"{a.DocumentId,-15} | {a.FullName,-25} | {a.PhoneNumber,-15} | ${a.GetBalance(),14:N0}");
            }
        }

        private static void SearchByDocumentMenu()
        {
            Console.Write("Ingrese el documento a buscar: ");
            string doc = Console.ReadLine();
            var associate = CooperativeService.FindByDocument(doc);

            if (associate == null)
            {
                Console.WriteLine("No se encontró ningún asociado con ese documento.");
                return;
            }

            ShowAssociateDetails(associate);
        }

        private static void SearchByNameMenu()
        {
            Console.Write("Ingrese el nombre (o parte de él): ");
            string query = Console.ReadLine();
            var results = CooperativeService.FindByName(query);

            if (results.Count == 0)
            {
                Console.WriteLine("No se encontraron coincidencias.");
                return;
            }

            Console.WriteLine($"\nCoincidencias encontradas: {results.Count}");
            foreach (var a in results)
            {
                ShowAssociateDetails(a);
            }
        }

        private static void ShowAssociateDetails(Associate a)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Documento: {a.DocumentId}");
            Console.WriteLine($"Nombre:    {a.FullName}");
            Console.WriteLine($"Teléfono:  {a.PhoneNumber}");
            Console.WriteLine($"Dirección: {a.Address}");
            Console.WriteLine($"Saldo:     ${a.GetBalance():N0} COP");
            Console.WriteLine($"Movimientos registrados: {a.Transactions.Count}");
            Console.WriteLine("------------------------------------------");
        }

        private static void UpdateAssociateMenu()
        {
            Console.Write("Documento del asociado a modificar: ");
            string doc = Console.ReadLine();
            var associate = CooperativeService.FindByDocument(doc);

            if (associate == null)
            {
                Console.WriteLine("Asociado no encontrado.");
                return;
            }

            Console.WriteLine("Deje el campo en blanco si no desea modificarlo:");
            Console.Write($"Nuevo Nombre [{associate.FullName}]: ");
            string name = Console.ReadLine();
            Console.Write($"Nuevo Teléfono [{associate.PhoneNumber}]: ");
            string phone = Console.ReadLine();
            Console.Write($"Nueva Dirección [{associate.Address}]: ");
            string address = Console.ReadLine();

            string error;
            bool success = CooperativeService.UpdateAssociate(doc, name, phone, address, out error);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✔ Datos actualizados correctamente.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✖ Error: {error}");
            }
            Console.ResetColor();
        }

        private static void DeleteAssociateMenu()
        {
            Console.Write("Documento del asociado a eliminar: ");
            string doc = Console.ReadLine();

            string error;
            bool success = CooperativeService.DeleteAssociate(doc, out error);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✔ Asociado eliminado satisfactoriamente.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✖ Rechazado: {error}");
            }
            Console.ResetColor();
        }

        private static void CheckBalanceCopMenu()
        {
            Console.Write("Documento del asociado: ");
            string doc = Console.ReadLine();
            var associate = CooperativeService.FindByDocument(doc);

            if (associate == null)
            {
                Console.WriteLine("Asociado no encontrado.");
                return;
            }

            Console.WriteLine($"\nAsociado: {associate.FullName}");
            Console.WriteLine($"Saldo Actual: ${associate.GetBalance():N0} COP");
        }

        private static async Task CheckBalanceUsdMenu()
        {
            Console.Write("Documento del asociado: ");
            string doc = Console.ReadLine();
            var associate = CooperativeService.FindByDocument(doc);

            if (associate == null)
            {
                Console.WriteLine("Asociado no encontrado.");
                return;
            }

            decimal balanceCop = associate.GetBalance();
            Console.WriteLine($"\nAsociado: {associate.FullName}");
            Console.WriteLine($"Saldo en COP: ${balanceCop:N0}");
            Console.WriteLine("\nConsultando TRM oficial con la Superintendencia Financiera...");

            var trm = await TrmService.GetCurrentTrmAsync();

            if (trm == null || trm.Value <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Aviso: No fue posible conectar con el servicio de la TRM. El sistema continuará operando.");
                Console.ResetColor();
                return;
            }

            decimal balanceUsd = balanceCop / trm.Value;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- CONVERSIÓN OFICIAL ---");
            Console.WriteLine($"Tasa TRM aplicada:   ${trm.Value:N2} COP / USD");
            Console.WriteLine($"Vigencia desde:      {trm.ValidFrom}");
            Console.WriteLine($"Vigencia hasta:      {trm.ValidTo}");
            Console.WriteLine($"Saldo en Dólares:    ${balanceUsd:N2} USD");
            Console.ResetColor();
        }

        private static void ProcessDepositMenu()
        {
            Console.Write("Documento del asociado: ");
            string doc = Console.ReadLine();
            Console.Write("Monto a consignar: $");
            string amountInput = Console.ReadLine();

            decimal amount;
            bool parsed = decimal.TryParse(amountInput, out amount);

            if (!parsed)
            {
                Console.WriteLine("Monto inválido.");
                return;
            }

            string error;
            bool success = CooperativeService.ProcessDeposit(doc, amount, out error);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✔ Consignación exitosa por ${amount:N0}.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✖ Transacción rechazada: {error}");
            }
            Console.ResetColor();
        }

        private static void ProcessWithdrawalMenu()
        {
            Console.Write("Documento del asociado: ");
            string doc = Console.ReadLine();
            Console.Write("Monto a retirar: $");
            string amountInput = Console.ReadLine();

            decimal amount;
            bool parsed = decimal.TryParse(amountInput, out amount);

            if (!parsed)
            {
                Console.WriteLine("Monto inválido.");
                return;
            }

            string error;
            bool success = CooperativeService.ProcessWithdrawal(doc, amount, out error);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✔ Retiro exitoso por ${amount:N0}.");
                if (amount > 1000000)
                {
                    Console.WriteLine("Nota: Se aplicó comisión de $8.000 por manejo de efectivo (retiro superior a $1.000.000).");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✖ Transacción rechazada: {error}");
            }
            Console.ResetColor();
        }

        private static void ViewTransactionsMenu()
        {
            Console.Write("Documento del asociado: ");
            string doc = Console.ReadLine();
            var associate = CooperativeService.FindByDocument(doc);

            if (associate == null)
            {
                Console.WriteLine("Asociado no encontrado.");
                return;
            }

            Console.WriteLine($"\nHistorial de Movimientos de: {associate.FullName}");

            if (associate.Transactions.Count == 0)
            {
                Console.WriteLine("No registra movimientos.");
                return;
            }

            Console.WriteLine($"{"Fecha",-20} | {"Tipo",-12} | {"Monto",14} | {"Comisión",10} | {"Total Débito/Crédito",20}");
            Console.WriteLine(new string('-', 85));

            foreach (var t in associate.Transactions)
            {
                string typeLabel;
                if (t.Type == TransactionType.Deposit)
                {
                    typeLabel = "Consignación";
                }
                else
                {
                    typeLabel = "Retiro";
                }

                Console.WriteLine($"{t.TransactionDate.ToString("yyyy-MM-dd HH:mm"),-20} | {typeLabel,-12} | ${t.Amount,13:N0} | ${t.Fee,9:N0} | ${t.TotalDeduction,19:N0}");
            }

            Console.WriteLine($"\nSaldo Final Calculado: ${associate.GetBalance():N0} COP");
        }

        private static async Task ReportsMenu()
        {
            Console.WriteLine("================ INFORMES DE GERENCIA ================");
            Console.WriteLine("1. ¿Cuánta plata tenemos?");
            Console.WriteLine("2. ¿Quiénes son mis mejores asociados?");
            Console.WriteLine("3. ¿Quiénes están dormidos?");
            Console.WriteLine("4. ¿Cómo nos fue en un periodo?");
            Console.WriteLine("5. ¿Cuáles fueron los movimientos más grandes?");
            Console.WriteLine("6. ¿Quién me está moviendo la caja?");
            Console.WriteLine("======================================================");
            Console.Write("Seleccione un informe: ");

            string rep = Console.ReadLine();
            Console.Clear();

            if (rep == "1")
            {
                var report = CooperativeService.GetGeneralBalanceReport();
                Console.WriteLine("--- 1. ¿CUÁNTA PLATA TENEMOS? ---");
                Console.WriteLine($"Saldo total de la cooperativa: ${report.TotalBalance:N0} COP");
                Console.WriteLine($"Cantidad de asociados:         {report.TotalAssociates}");
                Console.WriteLine($"Saldo promedio por asociado:   ${report.AverageBalance:N0} COP");
            }
            else if (rep == "2")
            {
                Console.WriteLine("--- 2. ¿QUIÉNES SON MIS MEJORES ASOCIADOS? (TOP 5) ---");
                var top5 = CooperativeService.GetTop5Associates();

                if (top5.Count == 0)
                {
                    Console.WriteLine("No hay asociados registrados.");
                }

                foreach (var a in top5)
                {
                    Console.WriteLine($"Doc: {a.DocumentId,-12} | Nombre: {a.FullName,-25} | Saldo: ${a.GetBalance(),14:N0}");
                }
            }
            else if (rep == "3")
            {
                Console.WriteLine("--- 3. ¿QUIÉNES ESTÁN DORMIDOS? (Sin movimientos) ---");
                var dormant = CooperativeService.GetDormantAssociates();

                if (dormant.Count == 0)
                {
                    Console.WriteLine("Todos los asociados tienen al menos un movimiento.");
                }

                foreach (var a in dormant)
                {
                    Console.WriteLine($"Doc: {a.DocumentId,-12} | Nombre: {a.FullName,-25} | Tel: {a.PhoneNumber}");
                }
            }
            else if (rep == "4")
            {
                Console.WriteLine("--- 4. ¿CÓMO NOS FUE EN UN PERIODO? ---");
                Console.Write("Fecha inicial (yyyy-MM-dd): ");
                string startInput = Console.ReadLine();

                DateTime start;
                bool startOk = DateTime.TryParseExact(startInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start);

                if (!startOk)
                {
                    Console.WriteLine("Fecha inicial inválida.");
                    return;
                }

                Console.Write("Fecha final (yyyy-MM-dd): ");
                string endInput = Console.ReadLine();

                DateTime end;
                bool endOk = DateTime.TryParseExact(endInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end);

                if (!endOk)
                {
                    Console.WriteLine("Fecha final inválida.");
                    return;
                }

                var p = CooperativeService.GetPeriodReport(start, end);
                Console.WriteLine($"\nConsignaciones: {p.DepositCount} movimientos | Total: ${p.TotalDeposited:N0}");
                Console.WriteLine($"Retiros:        {p.WithdrawalCount} movimientos | Total: ${p.TotalWithdrawn:N0}");
                Console.WriteLine($"Diferencia neta (Entradas - Salidas): ${p.NetDifference:N0}");
            }
            else if (rep == "5")
            {
                Console.WriteLine("--- 5. ¿CUÁLES FUERON LOS MOVIMIENTOS MÁS GRANDES? (TOP 10) ---");
                var top10 = CooperativeService.GetTop10Transactions();

                if (top10.Count == 0)
                {
                    Console.WriteLine("No hay movimientos registrados.");
                }

                foreach (var item in top10)
                {
                    string typeLabel;
                    if (item.Movement.Type == TransactionType.Deposit)
                    {
                        typeLabel = "Consignación";
                    }
                    else
                    {
                        typeLabel = "Retiro";
                    }

                    Console.WriteLine($"{item.Movement.TransactionDate:yyyy-MM-dd} | {typeLabel,-12} | Valor: ${item.Movement.Amount,13:N0} | Asociado: {item.AssociateName}");
                }
            }
            else if (rep == "6")
            {
                Console.WriteLine("--- 6. ¿QUIÉN ME ESTÁ MOVIENDO LA CAJA? ---");
                var activity = CooperativeService.GetAssociateActivityReport();

                Console.WriteLine($"{"Nombre",-22} | {"Movs",5} | {"Total Consignado",16} | {"Total Retirado",16} | {"Saldo Actual",14}");
                Console.WriteLine(new string('-', 82));

                foreach (var act in activity)
                {
                    Console.WriteLine($"{act.Name,-22} | {act.MovementCount,5} | ${act.TotalDeposited,15:N0} | ${act.TotalWithdrawn,15:N0} | ${act.Balance,13:N0}");
                }
            }
            else
            {
                Console.WriteLine("Informe no reconocido.");
            }

            await Task.CompletedTask;
        }
    }
}
