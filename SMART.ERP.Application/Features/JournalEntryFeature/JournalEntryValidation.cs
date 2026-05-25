using System.Globalization;
using Ardalis.Specification;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.FiscalPeriodSpecification;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature
{
    /// <summary>Reglas de negocio compartidas para crear/editar/contabilizar asientos contables.</summary>
    public static class JournalEntryValidation
    {
        private const decimal Tolerance = 0.01m;

        /// <summary>Valida la partida doble: al menos 2 líneas, un solo lado por línea, montos no negativos y debe = haber.</summary>
        public static void ValidateBalance(IReadOnlyList<JournalEntryLineInput> lines)
        {
            if (lines == null || lines.Count < 2)
                throw new ApiException("El asiento debe tener al menos dos líneas (partidas).");

            foreach (var line in lines)
            {
                if (line.Debit < 0 || line.Credit < 0)
                    throw new ApiException("Los montos del debe y el haber no pueden ser negativos.");
                if (line.Debit > 0 && line.Credit > 0)
                    throw new ApiException("Cada línea debe tener monto solo en el debe o solo en el haber, no en ambos.");
                if (line.Debit == 0 && line.Credit == 0)
                    throw new ApiException("Cada línea debe tener un monto en el debe o en el haber.");
                if (line.CustomerId.HasValue && line.ProviderId.HasValue)
                    throw new ApiException("Una línea no puede tener cliente y proveedor a la vez; elija solo uno.");
            }

            var totalDebit = lines.Sum(l => l.Debit);
            var totalCredit = lines.Sum(l => l.Credit);
            if (totalDebit <= 0)
                throw new ApiException("El total del asiento debe ser mayor a cero.");
            if (Math.Abs(totalDebit - totalCredit) > Tolerance)
                throw new ApiException($"El asiento no está balanceado: el total del debe ({totalDebit:N2}) debe ser igual al total del haber ({totalCredit:N2}).");
        }

        /// <summary>Verifica que todas las cuentas referenciadas existan, sean imputables y estén activas.</summary>
        public static async Task ValidatePostableAccountsAsync(
            IReadRepositoryAsync<LedgerAccount> accountRepository,
            IReadOnlyList<JournalEntryLineInput> lines,
            CancellationToken cancellationToken)
        {
            foreach (var accountId in lines.Select(l => l.LedgerAccountId).Distinct())
            {
                var account = await accountRepository.GetByIdAsync(accountId, cancellationToken)
                    ?? throw new ApiException($"No existe una cuenta contable con el Id {accountId}.");
                if (!account.IsPostable)
                    throw new ApiException($"La cuenta {account.Code} - {account.Name} no admite movimientos (no es imputable).");
                if (!account.IsActive)
                    throw new ApiException($"La cuenta {account.Code} - {account.Name} está inactiva.");
            }
        }

        /// <summary>Resuelve el período fiscal abierto para la fecha; lanza si no existe o está cerrado.</summary>
        public static async Task<FiscalPeriod> ResolveOpenPeriodAsync(
            IReadRepositoryAsync<FiscalPeriod> periodRepository,
            DateTime date,
            CancellationToken cancellationToken)
        {
            var period = await periodRepository.FirstOrDefaultAsync(new FilterFiscalPeriodByDateSpecification(date), cancellationToken)
                ?? throw new ApiException($"No existe un período fiscal para la fecha {date:dd/MM/yyyy}. Cree el ejercicio fiscal correspondiente.");
            if (period.Status == FiscalPeriodStatus.Closed)
                throw new ApiException($"El período {period.Name} está cerrado; no se permiten asientos en esa fecha.");
            return period;
        }

        /// <summary>Genera el siguiente número correlativo del año con formato "{year}-{seq:000000}".</summary>
        public static async Task<string> GenerateEntryNumberAsync(
            IReadRepositoryBase<JournalEntry> repository, int year, CancellationToken cancellationToken)
        {
            var last = await repository.FirstOrDefaultAsync(new FilterJournalEntryByYearPrefixSpecification(year), cancellationToken);
            var nextSequence = 1;
            if (last?.EntryNumber != null)
            {
                var parts = last.EntryNumber.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var seq))
                    nextSequence = seq + 1;
            }
            return $"{year}-{nextSequence:000000}";
        }
    }
}
