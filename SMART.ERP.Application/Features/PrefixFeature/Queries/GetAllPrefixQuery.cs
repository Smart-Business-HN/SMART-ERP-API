using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.PrefixFeature.Queries
{
    public class GetAllPrefixQuery : IRequest<PagedResponse<List<PrefixDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllPrefixQueryHandler : IRequestHandler<GetAllPrefixQuery, PagedResponse<List<PrefixDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Prefix> _repositoryAsync;
        public GetAllPrefixQueryHandler(IMapper mapper, IRepositoryAsync<Prefix> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<PagedResponse<List<PrefixDto>>> Handle(GetAllPrefixQuery request, CancellationToken cancellationToken)
        {
            if(request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var prefixes = await _repositoryAsync.ListAsync(new FilterAndPaginationPrefixSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<PrefixDto>>(prefixes);
            return new PagedResponse<List<PrefixDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

        }
    }
}
