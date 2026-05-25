using MediatR;
using SMART.ERP.Application.DTOs.AccountingReport;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingReportSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingReportFeature.Queries
{
    /// <summary>Libro Mayor: movimientos y saldo de una cuenta imputable en un rango de fechas.</summary>
    public class LibroMayorQuery : IRequest<Response<LibroMayorDto>>
    {
        public int LedgerAccountId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public class LibroMayorQueryHandler : IRequestHandler<LibroMayorQuery, Response<LibroMayorDto>>
        {
            private static readonly DateTime Epoch = new(1900, 1, 1);
            private readonly IReadRepositoryAsync<JournalEntryLine> _lineRepository;
            private readonly IReadRepositoryAsync<LedgerAccount> _accountRepository;

            public LibroMayorQueryHandler(IReadRepositoryAsync<JournalEntryLine> lineRepository, IReadRepositoryAsync<LedgerAccount> accountRepository)
            {
                _lineRepository = lineRepository;
                _accountRepository = accountRepository;
            }

            public async Task<Response<LibroMayorDto>> Handle(LibroMayorQuery request, CancellationToken cancellationToken)
            {
                var account = await _accountRepository.GetByIdAsync(request.LedgerAccountId, cancellationToken)
                    ?? throw new ApiException($"No existe una cuenta contable con el Id {request.LedgerAccountId}.");

                // Saldo de apertura: acumulado (debe - haber) antes de la fecha inicial.
                var priorLines = await _lineRepository.ListAsync(
                    new FilterPostedLinesByDateRangeSpecification(Epoch, request.FromDate.Date.AddDays(-1), request.LedgerAccountId), cancellationToken);
                var openingBalance = priorLines.Sum(l => l.Debit - l.Credit);

                var lines = await _lineRepository.ListAsync(
                    new FilterPostedLinesByDateRangeSpecification(request.FromDate, request.ToDate, request.LedgerAccountId), cancellationToken);

                var dto = new LibroMayorDto
                {
                    LedgerAccountId = account.Id,
                    AccountCode = account.Code,
                    AccountName = account.Name,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    OpeningBalance = openingBalance
                };

                var running = openingBalance;
                foreach (var line in lines)
                {
                    running += line.Debit - line.Credit;
                    dto.Movements.Add(new LibroMayorLineDto
                    {
                        EntryDate = line.JournalEntry!.EntryDate,
                        EntryNumber = line.JournalEntry.EntryNumber,
                        JournalEntryId = line.JournalEntryId,
                        Description = string.IsNullOrWhiteSpace(line.Description) ? line.JournalEntry.Description : line.Description!,
                        Debit = line.Debit,
                        Credit = line.Credit,
                        RunningBalance = running
                    });
                }

                dto.TotalDebit = lines.Sum(l => l.Debit);
                dto.TotalCredit = lines.Sum(l => l.Credit);
                dto.ClosingBalance = running;

                return new Response<LibroMayorDto>(dto);
            }
        }
    }
}
