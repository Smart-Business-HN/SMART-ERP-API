using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.StatusFeature.Queries
{
    public class GetAllStatusesQuery : IRequest<PagedResponse<List<StatusDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllStatusesQueryHandler : IRequestHandler<GetAllStatusesQuery, PagedResponse<List<StatusDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Status> _repositoryAsync;
            private readonly IRepositoryAsync<TypeStatus> _typeStatusRepositoryAsync;

            public GetAllStatusesQueryHandler(IMapper mapper, IRepositoryAsync<Status> repositoryAsync,
                 IRepositoryAsync<TypeStatus> typeStatusRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _typeStatusRepositoryAsync = typeStatusRepositoryAsync;
            }
            public async Task<PagedResponse<List<StatusDto>>> Handle(GetAllStatusesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var statuses = await _repositoryAsync.ListAsync(new FilterAndPaginationStatusSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<StatusDto>>(statuses);
                foreach (var item in dto)
                {
                    var typeStatus = await _typeStatusRepositoryAsync.GetByIdAsync(item.TypeStatusId);
                    if (typeStatus != null)
                        item.TypeStatus = _mapper.Map<ResumeTypeStatusDto>(typeStatus);
                }
                return new PagedResponse<List<StatusDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
