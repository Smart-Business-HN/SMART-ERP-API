using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.JournalEntryFeature;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.AccountingReportSpecification;
using SMART.ERP.Application.Specifications.FiscalPeriodSpecification;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.CloseFiscalYearCommand
{
    /// <summary>
    /// Cierra el ejercicio fiscal: genera el asiento de cierre que traslada el saldo de las cuentas
    /// de resultado (Ingresos, Costos, Gastos) a la cuenta de patrimonio "Utilidad o Pérdida del Período",
    /// cierra los 12 períodos y marca el ejercicio como cerrado.
    /// </summary>
    public class CloseFiscalYearCommand : IRequest<Response<JournalEntryDto>>
    {
        public int FiscalYearId { get; set; }

        public class CloseFiscalYearCommandHandler : IRequestHandler<CloseFiscalYearCommand, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRepositoryAsync<FiscalYear> _yearRepository;
            private readonly IRepositoryAsync<FiscalPeriod> _periodRepository;
            private readonly IRepositoryAsync<JournalEntry> _journalRepository;
            private readonly IReadRepositoryAsync<JournalEntryLine> _lineRepository;
            private readonly IReadRepositoryAsync<LedgerAccount> _accountRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public CloseFiscalYearCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IRepositoryAsync<FiscalYear> yearRepository, IRepositoryAsync<FiscalPeriod> periodRepository,
                IRepositoryAsync<JournalEntry> journalRepository, IReadRepositoryAsync<JournalEntryLine> lineRepository,
                IReadRepositoryAsync<LedgerAccount> accountRepository, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _yearRepository = yearRepository;
                _periodRepository = periodRepository;
                _journalRepository = journalRepository;
                _lineRepository = lineRepository;
                _accountRepository = accountRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<JournalEntryDto>> Handle(CloseFiscalYearCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                int closingEntryId = 0;

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var year = await _yearRepository.FirstOrDefaultAsync(new FilterFiscalYearByIdSpecification(request.FiscalYearId, asTracking: true), ct)
                        ?? throw new ApiException($"No existe un ejercicio fiscal con el Id {request.FiscalYearId}.");

                    if (year.IsClosed)
                        throw new ApiException($"El ejercicio {year.Year} ya está cerrado.");

                    var drafts = await _journalRepository.CountAsync(new FilterDraftJournalEntriesByYearSpecification(year.StartDate, year.EndDate), ct);
                    if (drafts > 0)
                        throw new ApiException("No se puede cerrar el ejercicio: existen asientos en borrador dentro del período. Contabilícelos o elimínelos primero.");

                    var resultAccount = await _accountRepository.FirstOrDefaultAsync(new FilterSystemResultAccountSpecification(), ct)
                        ?? throw new ApiException("No se encontró la cuenta de patrimonio del sistema para el resultado del ejercicio (Utilidad o Pérdida del Período).");

                    // Saldos de las cuentas de resultado (Ingresos, Costos, Gastos)
                    var lines = await _lineRepository.ListAsync(new FilterPostedLinesByDateRangeSpecification(year.StartDate, year.EndDate), ct);
                    var resultBalances = lines
                        .Where(l => l.LedgerAccount != null &&
                            (l.LedgerAccount.AccountType == AccountType.Ingresos
                          || l.LedgerAccount.AccountType == AccountType.Costos
                          || l.LedgerAccount.AccountType == AccountType.Gastos))
                        .GroupBy(l => l.LedgerAccountId)
                        .Select(g => new { LedgerAccountId = g.Key, Balance = g.Sum(x => x.Debit) - g.Sum(x => x.Credit) })
                        .Where(x => x.Balance != 0)
                        .ToList();

                    if (resultBalances.Count == 0)
                        throw new ApiException("No hay movimientos en cuentas de resultado para cerrar en este ejercicio.");

                    // El período de la fecha de cierre (31/dic) debe estar abierto al momento de postear.
                    var closingPeriod = await _periodRepository.FirstOrDefaultAsync(new FilterFiscalPeriodByDateSpecification(year.EndDate), ct)
                        ?? throw new ApiException("No se encontró el período de diciembre para registrar el asiento de cierre.");

                    var closingLines = new List<JournalEntryLine>();
                    var lineNumber = 1;
                    decimal netResult = 0m; // positivo = pérdida (saldo deudor neto), negativo = utilidad

                    foreach (var rb in resultBalances)
                    {
                        // Para anular: si la cuenta tiene saldo deudor (Balance>0) se acredita; si acreedor (Balance<0) se debita.
                        if (rb.Balance > 0)
                            closingLines.Add(new JournalEntryLine { LedgerAccountId = rb.LedgerAccountId, LineNumber = lineNumber++, Debit = 0, Credit = rb.Balance });
                        else
                            closingLines.Add(new JournalEntryLine { LedgerAccountId = rb.LedgerAccountId, LineNumber = lineNumber++, Debit = -rb.Balance, Credit = 0 });
                        netResult += rb.Balance;
                    }

                    // Contrapartida en patrimonio. netResult>0 => pérdida (debita patrimonio); netResult<0 => utilidad (acredita patrimonio).
                    if (netResult > 0)
                        closingLines.Add(new JournalEntryLine { LedgerAccountId = resultAccount.Id, LineNumber = lineNumber++, Debit = netResult, Credit = 0 });
                    else
                        closingLines.Add(new JournalEntryLine { LedgerAccountId = resultAccount.Id, LineNumber = lineNumber++, Debit = 0, Credit = -netResult });

                    var closingEntry = new JournalEntry
                    {
                        EntryDate = year.EndDate,
                        FiscalPeriodId = closingPeriod.Id,
                        Description = $"Asiento de cierre del ejercicio {year.Year}",
                        Reference = $"CIERRE-{year.Year}",
                        Status = JournalEntryStatus.Posted,
                        Source = JournalEntrySource.YearEndClosing,
                        TotalDebit = closingLines.Sum(l => l.Debit),
                        TotalCredit = closingLines.Sum(l => l.Credit),
                        EntryNumber = await JournalEntryValidation.GenerateEntryNumberAsync(_journalRepository, year.Year, ct),
                        PostedDate = DateTime.Now,
                        PostedBy = userName,
                        CreationDate = DateTime.Now,
                        CreatedBy = userName,
                        Lines = closingLines
                    };

                    var savedEntry = await _journalRepository.AddAsync(closingEntry, ct);
                    closingEntryId = savedEntry.Id;

                    // Cerrar los 12 períodos
                    var periods = await _periodRepository.ListAsync(new FilterFiscalPeriodsByYearSpecification(year.Id), ct);
                    foreach (var period in periods)
                    {
                        if (period.Status != FiscalPeriodStatus.Closed)
                        {
                            period.Status = FiscalPeriodStatus.Closed;
                            period.ClosedDate = DateTime.Now;
                            period.ClosedBy = userName;
                            await _periodRepository.UpdateAsync(period, ct);
                        }
                    }

                    year.IsClosed = true;
                    year.Status = FiscalPeriodStatus.Closed;
                    year.ClosingJournalEntryId = savedEntry.Id;
                    year.ClosedDate = DateTime.Now;
                    year.ClosedBy = userName;
                    year.ModificationDate = DateTime.Now;
                    year.ModifiedBy = userName;
                    await _yearRepository.UpdateAsync(year, ct);
                }, cancellationToken);

                await _outputCacheStored.EvictByTagAsync("cache_fiscal_periods", cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_accounting_reports", cancellationToken);

                var full = await _journalRepository.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(closingEntryId), cancellationToken);
                var dto = _mapper.Map<JournalEntryDto>(full);
                return new Response<JournalEntryDto>(dto, $"Ejercicio cerrado. Se generó el asiento de cierre {dto.EntryNumber}.");
            }
        }
    }
}
