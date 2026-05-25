using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.JournalEntry;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Queries
{
    /// <summary>Listado paginado del Libro Diario, con filtros de fecha y estado.</summary>
    public class GetAllJournalEntriesQuery : IRequest<PagedResponse<List<JournalEntryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public JournalEntryStatus? Status { get; set; }
        public bool All { get; set; }

        public class GetAllJournalEntriesQueryHandler : IRequestHandler<GetAllJournalEntriesQuery, PagedResponse<List<JournalEntryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;

            public GetAllJournalEntriesQueryHandler(IMapper mapper, IRepositoryAsync<JournalEntry> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<JournalEntryDto>>> Handle(GetAllJournalEntriesQuery request, CancellationToken cancellationToken)
            {
                var totalItems = await _repositoryAsync.CountAsync(
                    new FilterJournalEntryCountSpecification(request.Parameter, request.FromDate, request.ToDate, request.Status), cancellationToken);

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = totalItems == 0 ? 1 : totalItems;
                }

                var entries = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationJournalEntrySpecification(request.Parameter, request.PageNumber, request.PageSize,
                        request.FromDate, request.ToDate, request.Status),
                    cancellationToken);
                var dto = _mapper.Map<List<JournalEntryDto>>(entries);
                return new PagedResponse<List<JournalEntryDto>>(dto, request.PageNumber, request.PageSize, totalItems);
            }
        }
    }
}
