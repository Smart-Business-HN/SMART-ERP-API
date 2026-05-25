using MediatR;
using SMART.ERP.Application.DTOs.AccountingReport;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.AccountingReportFeature.Queries
{
    /// <summary>Estado de Resultados (Estado de Resultado Integral) para un rango de fechas.</summary>
    public class EstadoResultadosQuery : IRequest<Response<EstadoResultadosDto>>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public class EstadoResultadosQueryHandler : IRequestHandler<EstadoResultadosQuery, Response<EstadoResultadosDto>>
        {
            private readonly IReadRepositoryAsync<JournalEntryLine> _lineRepository;

            public EstadoResultadosQueryHandler(IReadRepositoryAsync<JournalEntryLine> lineRepository)
            {
                _lineRepository = lineRepository;
            }

            public async Task<Response<EstadoResultadosDto>> Handle(EstadoResultadosQuery request, CancellationToken cancellationToken)
            {
                var from = request.FromDate.Date;
                var to = request.ToDate.Date;
                var lines = await _lineRepository.ListAsync(new FilterPostedLinesByDateRangeSpecification(from, to), cancellationToken);

                var dto = new EstadoResultadosDto { FromDate = from, ToDate = to };

                var grouped = lines
                    .Where(l => l.LedgerAccount != null)
                    .GroupBy(l => l.LedgerAccountId)
                    .OrderBy(g => g.First().LedgerAccount!.Code);

                foreach (var group in grouped)
                {
                    var account = group.First().LedgerAccount!;
                    var debit = group.Sum(l => l.Debit);
                    var credit = group.Sum(l => l.Credit);

                    switch (account.AccountType)
                    {
                        case AccountType.Ingresos:
                            var ingreso = credit - debit;
                            if (ingreso != 0) dto.Ingresos.Add(Line(account, ingreso));
                            break;
                        case AccountType.Costos:
                            var costo = debit - credit;
                            if (costo != 0) dto.Costos.Add(Line(account, costo));
                            break;
                        case AccountType.Gastos:
                            var gasto = debit - credit;
                            if (gasto != 0) dto.Gastos.Add(Line(account, gasto));
                            break;
                    }
                }

                dto.TotalIngresos = dto.Ingresos.Sum(l => l.Amount);
                dto.TotalCostos = dto.Costos.Sum(l => l.Amount);
                dto.TotalGastos = dto.Gastos.Sum(l => l.Amount);
                dto.UtilidadBruta = dto.TotalIngresos - dto.TotalCostos;
                dto.UtilidadNeta = dto.TotalIngresos - dto.TotalCostos - dto.TotalGastos;

                return new Response<EstadoResultadosDto>(dto);
            }

            private static FinancialStatementLineDto Line(LedgerAccount account, decimal amount) => new()
            {
                LedgerAccountId = account.Id,
                AccountCode = account.Code,
                AccountName = account.Name,
                Amount = amount
            };
        }
    }
}
