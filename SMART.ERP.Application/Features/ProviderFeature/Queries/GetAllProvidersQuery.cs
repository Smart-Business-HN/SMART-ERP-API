using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Queries
{
    public class GetAllProvidersQuery : IRequest<PagedResponse<List<ProviderDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllProvidersQueryHandler : IRequestHandler<GetAllProvidersQuery, PagedResponse<List<ProviderDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Provider> _repositoryAsync;

            public GetAllProvidersQueryHandler(IMapper mapper, IRepositoryAsync<Provider> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<ProviderDto>>> Handle(GetAllProvidersQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var providers = await _repositoryAsync.ListAsync(new FilterAndPaginationProviderSpecification(
                    request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProviderDto>>(providers);
                return new PagedResponse<List<ProviderDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
