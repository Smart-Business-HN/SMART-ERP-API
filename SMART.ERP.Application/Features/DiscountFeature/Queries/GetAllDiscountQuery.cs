using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Discount;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DiscountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DiscountFeature.Queries
{
    public class GetAllDiscountQuery : IRequest<PagedResponse<List<DiscountDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllDiscountQueryHandler : IRequestHandler<GetAllDiscountQuery, PagedResponse<List<DiscountDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Discount> _repositoryAsync;
            
            public GetAllDiscountQueryHandler(IMapper mapper, IRepositoryAsync<Discount> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<DiscountDto>>> Handle(GetAllDiscountQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var departments = await _repositoryAsync.ListAsync(new FilterAndPaginationDiscountSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<DiscountDto>>(departments);
                return new PagedResponse<List<DiscountDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    
}

