using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.IncomeAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.IncomeAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.IncomeAccountFeature.Queries
{
    public class GetAllIncomeAccountQuery : IRequest<PagedResponse<List<IncomeAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllIncomeAccountQueryHandler : IRequestHandler<GetAllIncomeAccountQuery, PagedResponse<List<IncomeAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<IncomeAccount> _repositoryAsync;
            public GetAllIncomeAccountQueryHandler(IMapper mapper, IRepositoryAsync<IncomeAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<IncomeAccountDto>>> Handle(GetAllIncomeAccountQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var majorExpenseAccounts = await _repositoryAsync.ListAsync(new FilterAndPaginationIncomeAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<IncomeAccountDto>>(majorExpenseAccounts);
                return new PagedResponse<List<IncomeAccountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
