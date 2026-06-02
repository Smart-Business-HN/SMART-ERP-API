using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.JournalEntryFeature;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.JournalEntryLineSpecification;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.UpdateJournalEntryCommand
{
    /// <summary>Edita un asiento. Solo permitido mientras está en estado Borrador (inmutabilidad legal).</summary>
    public class UpdateJournalEntryCommand : IRequest<Response<JournalEntryDto>>
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string Description { get; set; } = null!;
        public string? Reference { get; set; }
        public List<JournalEntryLineInput> Lines { get; set; } = new();

        public class UpdateJournalEntryCommandHandler : IRequestHandler<UpdateJournalEntryCommand, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;
            private readonly IRepositoryAsync<JournalEntryLine> _lineRepository;
            private readonly IReadRepositoryAsync<LedgerAccount> _accountRepository;
            private readonly IReadRepositoryAsync<FiscalPeriod> _periodRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public UpdateJournalEntryCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IRepositoryAsync<JournalEntry> repositoryAsync, IRepositoryAsync<JournalEntryLine> lineRepository,
                IReadRepositoryAsync<LedgerAccount> accountRepository, IReadRepositoryAsync<FiscalPeriod> periodRepository,
                IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _repositoryAsync = repositoryAsync;
                _lineRepository = lineRepository;
                _accountRepository = accountRepository;
                _periodRepository = periodRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<JournalEntryDto>> Handle(UpdateJournalEntryCommand request, CancellationToken cancellationToken)
            {
                JournalEntryValidation.ValidateBalance(request.Lines);
                await JournalEntryValidation.ValidatePostableAccountsAsync(_accountRepository, request.Lines, cancellationToken);

                var userName = _jwtService.GetSubjectToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var entry = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id, asTracking: true), ct)
                        ?? throw new ApiException($"No existe un asiento con el Id {request.Id}.");

                    if (entry.Status != JournalEntryStatus.Draft)
                        throw new ApiException("Solo se pueden editar asientos en estado Borrador. Un asiento contabilizado solo puede reversarse.");

                    var period = await JournalEntryValidation.ResolveOpenPeriodAsync(_periodRepository, request.EntryDate, ct);

                    // Reemplazar las líneas existentes
                    var existingLines = await _lineRepository.ListAsync(new FilterJournalEntryLinesByEntrySpecification(entry.Id), ct);
                    if (existingLines.Count > 0)
                        await _lineRepository.DeleteRangeAsync(existingLines, ct);

                    var lineNumber = 1;
                    entry.EntryDate = request.EntryDate;
                    entry.FiscalPeriodId = period.Id;
                    entry.Description = request.Description.Trim();
                    entry.Reference = request.Reference;
                    entry.TotalDebit = request.Lines.Sum(l => l.Debit);
                    entry.TotalCredit = request.Lines.Sum(l => l.Credit);
                    entry.ModificationDate = DateTime.Now;
                    entry.ModifiedBy = userName;
                    entry.Lines = request.Lines.Select(l => new JournalEntryLine
                    {
                        JournalEntryId = entry.Id,
                        LedgerAccountId = l.LedgerAccountId,
                        LineNumber = lineNumber++,
                        Debit = l.Debit,
                        Credit = l.Credit,
                        Description = l.Description,
                        ProjectId = l.ProjectId,
                        CustomerId = l.CustomerId,
                        ProviderId = l.ProviderId
                    }).ToList();

                    // NoTracking global: desligamos FiscalPeriod para no arrastrarla en el Update.
                    // Las líneas nuevas (sin Id, en entry.Lines) se insertan vía el Update de la cabecera.
                    entry.FiscalPeriod = null;
                    await _repositoryAsync.UpdateAsync(entry, ct);
                }, cancellationToken);

                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<JournalEntryDto>(full);
                return new Response<JournalEntryDto>(dto, "Asiento actualizado exitosamente.");
            }
        }
    }
}
