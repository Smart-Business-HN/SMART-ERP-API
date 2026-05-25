using MediatR;
using SMART.ERP.Application.DTOs.AccountingReport;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.AccountingReportFeature.Queries
{
    /// <summary>
    /// Estado de Situación Financiera (Balance General) a una fecha de corte. El resultado del
    /// período aún no cerrado (Ingresos - Costos - Gastos) se incluye dentro del patrimonio.
    /// </summary>
    public class EstadoSituacionFinancieraQuery : IRequest<Response<EstadoSituacionFinancieraDto>>
    {
        public DateTime CutoffDate { get; set; }

        public class EstadoSituacionFinancieraQueryHandler : IRequestHandler<EstadoSituacionFinancieraQuery, Response<EstadoSituacionFinancieraDto>>
        {
            private static readonly DateTime Epoch = new(1900, 1, 1);
            private readonly IReadRepositoryAsync<JournalEntryLine> _lineRepository;

            public EstadoSituacionFinancieraQueryHandler(IReadRepositoryAsync<JournalEntryLine> lineRepository)
            {
                _lineRepository = lineRepository;
            }

            public async Task<Response<EstadoSituacionFinancieraDto>> Handle(EstadoSituacionFinancieraQuery request, CancellationToken cancellationToken)
            {
                var cutoff = request.CutoffDate.Date;
                var lines = await _lineRepository.ListAsync(new FilterPostedLinesByDateRangeSpecification(Epoch, cutoff), cancellationToken);

                var dto = new EstadoSituacionFinancieraDto { CutoffDate = cutoff };
                decimal resultado = 0m;

                var grouped = lines
                    .Where(l => l.LedgerAccount != null)
                    .GroupBy(l => l.LedgerAccountId)
                    .OrderBy(g => g.First().LedgerAccount!.Code);

                foreach (var group in grouped)
                {
                    var account = group.First().LedgerAccount!;
                    var debit = group.Sum(l => l.Debit);
                    var credit = group.Sum(l => l.Credit);
                    var netDebit = debit - credit;   // saldo deudor positivo
                    var netCredit = credit - debit;  // saldo acreedor positivo

                    switch (account.AccountType)
                    {
                        case AccountType.Activo:
                            if (netDebit != 0) dto.Activos.Add(Line(account, netDebit));
                            break;
                        case AccountType.Pasivo:
                            if (netCredit != 0) dto.Pasivos.Add(Line(account, netCredit));
                            break;
                        case AccountType.Patrimonio:
                            if (netCredit != 0) dto.Patrimonio.Add(Line(account, netCredit));
                            break;
                        case AccountType.Ingresos:
                            resultado += netCredit;
                            break;
                        case AccountType.Costos:
                        case AccountType.Gastos:
                            resultado -= netDebit;
                            break;
                    }
                }

                dto.TotalActivos = dto.Activos.Sum(l => l.Amount);
                dto.TotalPasivos = dto.Pasivos.Sum(l => l.Amount);
                dto.ResultadoDelPeriodo = resultado;
                dto.TotalPatrimonio = dto.Patrimonio.Sum(l => l.Amount) + resultado;
                dto.IsBalanced = Math.Abs(dto.TotalActivos - (dto.TotalPasivos + dto.TotalPatrimonio)) < 0.01m;

                return new Response<EstadoSituacionFinancieraDto>(dto);
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
