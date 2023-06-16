using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeOriginSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Queries
{
    public class GetAllTypeOriginQuery : IRequest<PagedResponse<List<TypeOriginDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllTypeOriginQueryHandler : IRequestHandler<GetAllTypeOriginQuery, PagedResponse<List<TypeOriginDto>>>
        {
            private readonly IRepositoryAsync<TypeOrigin> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllTypeOriginQueryHandler(IRepositoryAsync<TypeOrigin> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<PagedResponse<List<TypeOriginDto>>> Handle(GetAllTypeOriginQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var originList = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationTypeOriginSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<TypeOriginDto>>(originList);
                return new PagedResponse<List<TypeOriginDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
