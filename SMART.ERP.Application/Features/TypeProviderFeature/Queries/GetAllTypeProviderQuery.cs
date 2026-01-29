using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.TypeProvider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeProviderFeature.Queries
{
    public class GetAllTypeProviderQuery : IRequest<PagedResponse<List<TypeProviderDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllTypeProviderQueryHandler : IRequestHandler<GetAllTypeProviderQuery, PagedResponse<List<TypeProviderDto>>>
        {
            private readonly IRepositoryAsync<TypeProvider> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllTypeProviderQueryHandler(IRepositoryAsync<TypeProvider> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<PagedResponse<List<TypeProviderDto>>> Handle(GetAllTypeProviderQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var originList = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationTypeProviderSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<TypeProviderDto>>(originList);
                return new PagedResponse<List<TypeProviderDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
