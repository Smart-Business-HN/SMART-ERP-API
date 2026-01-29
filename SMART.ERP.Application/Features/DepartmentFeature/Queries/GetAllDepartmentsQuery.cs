using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DepartmentFeature.Queries
{
    public class GetAllDepartmentsQuery : IRequest<PagedResponse<List<DepartmentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PagedResponse<List<DepartmentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Department> _repositoryAsync;
            public GetAllDepartmentsQueryHandler(IMapper mapper, IRepositoryAsync<Department> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var departments = await _repositoryAsync.ListAsync(new FilterAndPaginationDepartmentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<DepartmentDto>>(departments);
                return new PagedResponse<List<DepartmentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
