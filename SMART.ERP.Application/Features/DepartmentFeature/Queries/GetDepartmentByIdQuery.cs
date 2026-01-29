using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DepartmentFeature.Queries
{
    public class GetDepartmentByIdQuery : IRequest<Response<DepartmentDto>>
    {
        public int Id { get; set; }
    }

    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Response<DepartmentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Department> _repositoryAsync;

        public GetDepartmentByIdQueryHandler(IMapper mapper, IRepositoryAsync<Department> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _repositoryAsync.FirstOrDefaultAsync(new DepartmentIncludesSpecification(id: request.Id));
            if (department == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<DepartmentDto>(department);
            return new Response<DepartmentDto>(dto);
        }
    }
}
