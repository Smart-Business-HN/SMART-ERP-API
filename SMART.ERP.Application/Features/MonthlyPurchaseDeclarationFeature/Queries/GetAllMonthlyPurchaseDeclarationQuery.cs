using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.MonthlyPurchaseDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MonthlyPurchaseDeclarationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Queries
{
    public class GetAllMonthlyPurchaseDeclarationQuery : IRequest<PagedResponse<List<MonthlyPurchaseDeclarationDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllMonthlyPurchaseDeclarationQueryHandler : IRequestHandler<GetAllMonthlyPurchaseDeclarationQuery, PagedResponse<List<MonthlyPurchaseDeclarationDto>>>
    {
        private readonly IRepositoryAsync<MonthlyPurchaseDeclaration> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllMonthlyPurchaseDeclarationQueryHandler(IRepositoryAsync<MonthlyPurchaseDeclaration> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<MonthlyPurchaseDeclarationDto>>> Handle(GetAllMonthlyPurchaseDeclarationQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var quotations = await _repositoryAsync.ListAsync(new GetAllMonthlyPurchaseDeclarationSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MonthlyPurchaseDeclarationDto>>(quotations);
            return new PagedResponse<List<MonthlyPurchaseDeclarationDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
