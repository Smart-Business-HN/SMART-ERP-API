using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.CreateJournalEntryCommand
{
    /// <summary>Crea un asiento en estado Borrador (Draft). No asigna número hasta contabilizarse.</summary>
    public class CreateJournalEntryCommand : IRequest<Response<JournalEntryDto>>
    {
        public DateTime EntryDate { get; set; }
        public string Description { get; set; } = null!;
        public string? Reference { get; set; }
        public List<JournalEntryLineInput> Lines { get; set; } = new();

        public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;
            private readonly IReadRepositoryAsync<LedgerAccount> _accountRepository;
            private readonly IReadRepositoryAsync<FiscalPeriod> _periodRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public CreateJournalEntryCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<JournalEntry> repositoryAsync, IReadRepositoryAsync<LedgerAccount> accountRepository,
                IReadRepositoryAsync<FiscalPeriod> periodRepository, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _accountRepository = accountRepository;
                _periodRepository = periodRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<JournalEntryDto>> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
            {
                JournalEntryValidation.ValidateBalance(request.Lines);
                await JournalEntryValidation.ValidatePostableAccountsAsync(_accountRepository, request.Lines, cancellationToken);
                var period = await JournalEntryValidation.ResolveOpenPeriodAsync(_periodRepository, request.EntryDate, cancellationToken);

                var userName = _jwtService.GetSubjectToken();
                var lineNumber = 1;
                var entry = new JournalEntry
                {
                    EntryDate = request.EntryDate,
                    FiscalPeriodId = period.Id,
                    Description = request.Description.Trim(),
                    Reference = request.Reference,
                    Status = JournalEntryStatus.Draft,
                    Source = JournalEntrySource.Manual,
                    TotalDebit = request.Lines.Sum(l => l.Debit),
                    TotalCredit = request.Lines.Sum(l => l.Credit),
                    CreationDate = DateTime.Now,
                    CreatedBy = userName,
                    Lines = request.Lines.Select(l => new JournalEntryLine
                    {
                        LedgerAccountId = l.LedgerAccountId,
                        LineNumber = lineNumber++,
                        Debit = l.Debit,
                        Credit = l.Credit,
                        Description = l.Description,
                        ProjectId = l.ProjectId,
                        CustomerId = l.CustomerId,
                        ProviderId = l.ProviderId
                    }).ToList()
                };

                var data = await _repositoryAsync.AddAsync(entry, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(data.Id), cancellationToken);
                var dto = _mapper.Map<JournalEntryDto>(full);
                return new Response<JournalEntryDto>(dto, "Asiento contable creado en borrador.");
            }
        }
    }
}
