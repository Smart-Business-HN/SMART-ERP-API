using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.JournalEntryFeature;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.ReverseJournalEntryCommand
{
    /// <summary>
    /// Reversa un asiento contabilizado generando un asiento espejo (debe/haber invertidos).
    /// El original queda en estado Reversed. Cumple la inmutabilidad del Libro Diario.
    /// </summary>
    public class ReverseJournalEntryCommand : IRequest<Response<JournalEntryDto>>
    {
        public int Id { get; set; }
        /// <summary>Fecha del asiento de reversa. Si no se indica, se usa la fecha actual.</summary>
        public DateTime? ReversalDate { get; set; }
        public string? Reason { get; set; }

        public class ReverseJournalEntryCommandHandler : IRequestHandler<ReverseJournalEntryCommand, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;
            private readonly IReadRepositoryAsync<FiscalPeriod> _periodRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public ReverseJournalEntryCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IRepositoryAsync<JournalEntry> repositoryAsync, IReadRepositoryAsync<FiscalPeriod> periodRepository,
                IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _repositoryAsync = repositoryAsync;
                _periodRepository = periodRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<JournalEntryDto>> Handle(ReverseJournalEntryCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                var reversalDate = (request.ReversalDate ?? DateTime.Now).Date;
                int reversalId = 0;

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var original = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id, asTracking: true), ct)
                        ?? throw new ApiException($"No existe un asiento con el Id {request.Id}.");

                    if (original.Status != JournalEntryStatus.Posted)
                        throw new ApiException("Solo se pueden reversar asientos contabilizados (Posted).");

                    var period = await JournalEntryValidation.ResolveOpenPeriodAsync(_periodRepository, reversalDate, ct);

                    var lineNumber = 1;
                    var reversal = new JournalEntry
                    {
                        EntryDate = reversalDate,
                        FiscalPeriodId = period.Id,
                        Description = $"Reversa del asiento {original.EntryNumber}" + (string.IsNullOrWhiteSpace(request.Reason) ? "" : $" - {request.Reason}"),
                        Reference = original.EntryNumber,
                        Status = JournalEntryStatus.ReversalEntry,
                        Source = original.Source,
                        ReversesJournalEntryId = original.Id,
                        TotalDebit = original.TotalCredit,
                        TotalCredit = original.TotalDebit,
                        EntryNumber = await JournalEntryValidation.GenerateEntryNumberAsync(_repositoryAsync, reversalDate.Year, ct),
                        PostedDate = DateTime.Now,
                        PostedBy = userName,
                        CreationDate = DateTime.Now,
                        CreatedBy = userName,
                        Lines = (original.Lines ?? new List<JournalEntryLine>())
                            .OrderBy(l => l.LineNumber)
                            .Select(l => new JournalEntryLine
                            {
                                LedgerAccountId = l.LedgerAccountId,
                                LineNumber = lineNumber++,
                                Debit = l.Credit,
                                Credit = l.Debit,
                                Description = l.Description,
                                ProjectId = l.ProjectId
                            }).ToList()
                    };

                    var saved = await _repositoryAsync.AddAsync(reversal, ct);
                    reversalId = saved.Id;

                    original.Status = JournalEntryStatus.Reversed;
                    original.ReversedByJournalEntryId = saved.Id;
                    original.ModificationDate = DateTime.Now;
                    original.ModifiedBy = userName;
                    // NoTracking global: desligamos el grafo (evita InvalidOperationException por
                    // terceros duplicados) y actualizamos solo la cabecera del asiento original.
                    original.Lines = null;
                    original.FiscalPeriod = null;
                    await _repositoryAsync.UpdateAsync(original, ct);
                }, cancellationToken);

                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_accounting_reports", cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(reversalId), cancellationToken);
                var dto = _mapper.Map<JournalEntryDto>(full);
                return new Response<JournalEntryDto>(dto, $"Asiento reversado. Se generó el asiento de reversa {dto.EntryNumber}.");
            }
        }
    }
}
