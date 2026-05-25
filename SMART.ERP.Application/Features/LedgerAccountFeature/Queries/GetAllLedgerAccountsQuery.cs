using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.LedgerAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Queries
{
    public class GetAllLedgerAccountsQuery : IRequest<PagedResponse<List<LedgerAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllLedgerAccountsQueryHandler : IRequestHandler<GetAllLedgerAccountsQuery, PagedResponse<List<LedgerAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;

            public GetAllLedgerAccountsQueryHandler(IMapper mapper, IRepositoryAsync<LedgerAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<LedgerAccountDto>>> Handle(GetAllLedgerAccountsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var accounts = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationLedgerAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
                    cancellationToken);
                var dto = _mapper.Map<List<LedgerAccountDto>>(accounts);
                return new PagedResponse<List<LedgerAccountDto>>(dto, request.PageNumber, request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}
