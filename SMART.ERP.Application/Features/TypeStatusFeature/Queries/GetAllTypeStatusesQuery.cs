using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeStatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Queries
{
    public class GetAllTypeStatusesQuery : IRequest<PagedResponse<List<TypeStatusDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllTypeStatusesQueryHandler : IRequestHandler<GetAllTypeStatusesQuery, PagedResponse<List<TypeStatusDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;
            public GetAllTypeStatusesQueryHandler(IMapper mapper, IRepositoryAsync<TypeStatus> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<TypeStatusDto>>> Handle(GetAllTypeStatusesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                if (request.Parameter == "")
                {
                    request.Parameter = null;
                }
                if (request.Order == "")
                {
                    request.Order = null;
                }
                if (request.Column == "")
                {
                    request.Column = null;
                }
                var typeStatuses = await _repositoryAsync.ListAsync(new FilterAndPaginationTypeStatusSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<TypeStatusDto>>(typeStatuses);
                return new PagedResponse<List<TypeStatusDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
