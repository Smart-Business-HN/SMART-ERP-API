using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Queries
{
    public class GetAllNonBillableExpensesQuery : IRequest<PagedResponse<List<NonBillableExpenseDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllNonBillableExpensesQueryHandler : IRequestHandler<GetAllNonBillableExpensesQuery, PagedResponse<List<NonBillableExpenseDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;

            public GetAllNonBillableExpensesQueryHandler(IMapper mapper, IRepositoryAsync<NonBillableExpense> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<NonBillableExpenseDto>>> Handle(GetAllNonBillableExpensesQuery request, CancellationToken cancellationToken)
            {

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var categories = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationNonBillableExpenseSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<NonBillableExpenseDto>>(categories);
                return new PagedResponse<List<NonBillableExpenseDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
