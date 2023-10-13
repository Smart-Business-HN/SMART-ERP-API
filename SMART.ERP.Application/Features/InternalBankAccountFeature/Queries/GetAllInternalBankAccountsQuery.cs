using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InternalBankAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Queries
{
    public class GetAllInternalBankAccountsQuery : IRequest<PagedResponse<List<InternalBankAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllInternalBankAccountsQueryHandler : IRequestHandler<GetAllInternalBankAccountsQuery, PagedResponse<List<InternalBankAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<InternalBankAccount> _repositoryAsync;
            public GetAllInternalBankAccountsQueryHandler(IMapper mapper, IRepositoryAsync<InternalBankAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<InternalBankAccountDto>>> Handle(GetAllInternalBankAccountsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var internalBankAccounts = await _repositoryAsync.ListAsync(new FilterAndPaginationInternalBankAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<InternalBankAccountDto>>(internalBankAccounts);
                return new PagedResponse<List<InternalBankAccountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

            }
        }
    }
}
