using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.DepartmentFeature.Queries
{
    public class GetAllDepartmentsFromHNQuery : IRequest<PagedResponse<List<ClientDepartmentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllDepartmentsFromHNQueryHandler : IRequestHandler<GetAllDepartmentsFromHNQuery, PagedResponse<List<ClientDepartmentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryHNAsync<ClientDepartment> _repositoryAsync;
            public GetAllDepartmentsFromHNQueryHandler(IMapper mapper, IRepositoryHNAsync<ClientDepartment> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<ClientDepartmentDto>>> Handle(GetAllDepartmentsFromHNQuery request, CancellationToken cancellationToken)
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
                var departments = await _repositoryAsync.ListAsync(new FilterAndPaginationClientDepartmentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ClientDepartmentDto>>(departments);
                return new PagedResponse<List<ClientDepartmentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
