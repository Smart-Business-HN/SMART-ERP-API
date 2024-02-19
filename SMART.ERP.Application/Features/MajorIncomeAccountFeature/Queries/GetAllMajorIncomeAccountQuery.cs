using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.MajorIncomeAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MajorIncomeAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Queries
{
    public class GetAllMajorIncomeAccountQuery : IRequest<PagedResponse<List<MajorIncomeAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllMajorIncomeAccountQueryHandler : IRequestHandler<GetAllMajorIncomeAccountQuery, PagedResponse<List<MajorIncomeAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<MajorIncomeAccount> _repositoryAsync;
            public GetAllMajorIncomeAccountQueryHandler(IMapper mapper, IRepositoryAsync<MajorIncomeAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<MajorIncomeAccountDto>>> Handle(GetAllMajorIncomeAccountQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var majorExpenseAccounts = await _repositoryAsync.ListAsync(new FilterAndPaginationMajorIncomeAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<MajorIncomeAccountDto>>(majorExpenseAccounts);
                return new PagedResponse<List<MajorIncomeAccountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
