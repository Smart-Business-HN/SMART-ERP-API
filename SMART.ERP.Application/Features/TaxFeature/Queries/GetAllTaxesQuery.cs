using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TaxSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TaxFeature.Queries
{
    public class GetAllTaxesQuery : IRequest<PagedResponse<List<TaxDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllTaxesQueryHandler : IRequestHandler<GetAllTaxesQuery, PagedResponse<List<TaxDto>>>
    {
        private readonly IRepositoryAsync<Tax> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllTaxesQueryHandler(IRepositoryAsync<Tax> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<TaxDto>>> Handle(GetAllTaxesQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var taxes = await _repositoryAsync.ListAsync(new FilterAndPaginationTaxSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<TaxDto>>(taxes);
            return new PagedResponse<List<TaxDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
