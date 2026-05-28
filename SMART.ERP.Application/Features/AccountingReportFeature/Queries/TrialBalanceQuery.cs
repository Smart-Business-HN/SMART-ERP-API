using MediatR;
using SMART.ERP.Application.DTOs.AccountingReport;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingReportFeature.Queries
{
    /// <summary>
    /// Balance de Comprobación: movimientos del período (Debe/Haber) y saldos acumulados
    /// (Deudor/Acreedor) por cuenta imputable. Los totales deben cuadrar.
    /// </summary>
    public class TrialBalanceQuery : IRequest<Response<TrialBalanceDto>>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public class TrialBalanceQueryHandler : IRequestHandler<TrialBalanceQuery, Response<TrialBalanceDto>>
        {
            private static readonly DateTime Epoch = new(1900, 1, 1);
            private readonly IReadRepositoryAsync<JournalEntryLine> _lineRepository;

            public TrialBalanceQueryHandler(IReadRepositoryAsync<JournalEntryLine> lineRepository)
            {
                _lineRepository = lineRepository;
            }

            public async Task<Response<TrialBalanceDto>> Handle(TrialBalanceQuery request, CancellationToken cancellationToken)
            {
                var from = request.FromDate.Date;
                var to = request.ToDate.Date;

                // Todas las partidas hasta la fecha de corte para acumular saldos.
                var allLines = await _lineRepository.ListAsync(new FilterPostedLinesByDateRangeSpecification(Epoch, to), cancellationToken);

                var dto = new TrialBalanceDto { FromDate = from, ToDate = to };

                var grouped = allLines
                    .Where(l => l.LedgerAccount != null)
                    .GroupBy(l => l.LedgerAccountId)
                    .OrderBy(g => g.First().LedgerAccount!.Code);

                foreach (var group in grouped)
                {
                    var account = group.First().LedgerAccount!;
                    var periodDebe = group.Where(l => l.JournalEntry!.EntryDate.Date >= from && l.JournalEntry.EntryDate.Date <= to).Sum(l => l.Debit);
                    var periodHaber = group.Where(l => l.JournalEntry!.EntryDate.Date >= from && l.JournalEntry.EntryDate.Date <= to).Sum(l => l.Credit);
                    var saldoNet = group.Sum(l => l.Debit - l.Credit);

                    if (periodDebe == 0 && periodHaber == 0 && saldoNet == 0)
                        continue;

                    dto.Lines.Add(new TrialBalanceLineDto
                    {
                        LedgerAccountId = account.Id,
                        AccountCode = account.Code,
                        AccountName = account.Name,
                        Debe = periodDebe,
                        Haber = periodHaber,
                        SaldoDeudor = saldoNet > 0 ? saldoNet : 0,
                        SaldoAcreedor = saldoNet < 0 ? -saldoNet : 0
                    });
                }

                dto.TotalDebe = dto.Lines.Sum(l => l.Debe);
                dto.TotalHaber = dto.Lines.Sum(l => l.Haber);
                dto.TotalSaldoDeudor = dto.Lines.Sum(l => l.SaldoDeudor);
                dto.TotalSaldoAcreedor = dto.Lines.Sum(l => l.SaldoAcreedor);
                dto.IsBalanced = Math.Abs(dto.TotalDebe - dto.TotalHaber) < 0.01m
                              && Math.Abs(dto.TotalSaldoDeudor - dto.TotalSaldoAcreedor) < 0.01m;

                return new Response<TrialBalanceDto>(dto);
            }
        }
    }
}
