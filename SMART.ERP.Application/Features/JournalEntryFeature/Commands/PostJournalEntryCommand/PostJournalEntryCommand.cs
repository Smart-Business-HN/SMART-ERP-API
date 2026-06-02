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

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.PostJournalEntryCommand
{
    /// <summary>Contabiliza (postea) un asiento en borrador: asigna el número correlativo y lo vuelve inmutable.</summary>
    public class PostJournalEntryCommand : IRequest<Response<JournalEntryDto>>
    {
        public int Id { get; set; }

        public class PostJournalEntryCommandHandler : IRequestHandler<PostJournalEntryCommand, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;
            private readonly IReadRepositoryAsync<LedgerAccount> _accountRepository;
            private readonly IReadRepositoryAsync<FiscalPeriod> _periodRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public PostJournalEntryCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IRepositoryAsync<JournalEntry> repositoryAsync, IReadRepositoryAsync<LedgerAccount> accountRepository,
                IReadRepositoryAsync<FiscalPeriod> periodRepository, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _repositoryAsync = repositoryAsync;
                _accountRepository = accountRepository;
                _periodRepository = periodRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<JournalEntryDto>> Handle(PostJournalEntryCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var entry = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id, asTracking: true), ct)
                        ?? throw new ApiException($"No existe un asiento con el Id {request.Id}.");

                    if (entry.Status != JournalEntryStatus.Draft)
                        throw new ApiException("Solo se pueden contabilizar asientos en estado Borrador.");

                    var lines = (entry.Lines ?? new List<JournalEntryLine>())
                        .Select(l => new JournalEntryLineInput { LedgerAccountId = l.LedgerAccountId, Debit = l.Debit, Credit = l.Credit })
                        .ToList();

                    JournalEntryValidation.ValidateBalance(lines);
                    await JournalEntryValidation.ValidatePostableAccountsAsync(_accountRepository, lines, ct);
                    await JournalEntryValidation.ResolveOpenPeriodAsync(_periodRepository, entry.EntryDate, ct);

                    entry.EntryNumber = await JournalEntryValidation.GenerateEntryNumberAsync(_repositoryAsync, entry.EntryDate.Year, ct);
                    entry.TotalDebit = lines.Sum(l => l.Debit);
                    entry.TotalCredit = lines.Sum(l => l.Credit);
                    entry.Status = JournalEntryStatus.Posted;
                    entry.PostedDate = DateTime.Now;
                    entry.PostedBy = userName;
                    entry.ModificationDate = DateTime.Now;
                    entry.ModifiedBy = userName;

                    // El asiento ya viene rastreado (asTracking:true) con sus navegaciones
                    // (LedgerAccount/Customer/Provider). Persistimos con SaveChanges para guardar
                    // solo los cambios del asiento; NO usar UpdateAsync, que haría Update sobre todo
                    // el grafo y marcaría como Modified las cuentas/cliente/proveedor maestros.
                    await _repositoryAsync.SaveChangesAsync(ct);
                }, cancellationToken);

                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_accounting_reports", cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<JournalEntryDto>(full);
                return new Response<JournalEntryDto>(dto, $"Asiento {dto.EntryNumber} contabilizado exitosamente.");
            }
        }
    }
}
