using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Queries
{
    public class GetJournalEntryByIdQuery : IRequest<Response<JournalEntryDto>>
    {
        public int Id { get; set; }

        public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, Response<JournalEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;

            public GetJournalEntryByIdQueryHandler(IMapper mapper, IRepositoryAsync<JournalEntry> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<JournalEntryDto>> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
            {
                var entry = await _repositoryAsync.FirstOrDefaultAsync(new FilterJournalEntryByIdSpecification(request.Id), cancellationToken)
                    ?? throw new KeyNotFoundException($"No existe un asiento con el Id {request.Id}.");
                var dto = _mapper.Map<JournalEntryDto>(entry);
                return new Response<JournalEntryDto>(dto);
            }
        }
    }
}
