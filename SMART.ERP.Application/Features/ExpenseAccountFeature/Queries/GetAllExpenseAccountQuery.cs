using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ExpenseAccount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ExpenseAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ExpenseAccountFeature.Queries
{
    public class GetAllExpenseAccountQuery : IRequest<PagedResponse<List<ExpenseAccountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllExpenseAccountQueryHandler : IRequestHandler<GetAllExpenseAccountQuery, PagedResponse<List<ExpenseAccountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<ExpenseAccount> _repositoryAsync;
            public GetAllExpenseAccountQueryHandler(IMapper mapper, IRepositoryAsync<ExpenseAccount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<ExpenseAccountDto>>> Handle(GetAllExpenseAccountQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var majorExpenseAccounts = await _repositoryAsync.ListAsync(new FilterAndPaginationExpenseAccountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ExpenseAccountDto>>(majorExpenseAccounts);
                return new PagedResponse<List<ExpenseAccountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
