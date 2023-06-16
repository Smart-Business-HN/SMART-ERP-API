using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.Features.DepartmentFeature.Commands.CreateDepartmentCommand
{
    public class CreateDepartmentCommand : IRequest<Response<DepartmentDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CountryId { get; set; }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Response<DepartmentDto>>
    {
        private readonly IRepositoryAsync<Department> _repositoryAsync;
        private readonly IRepositoryAsync<Country> _countryRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IRepositoryAsync<Department> repositoryAsync, IRepositoryAsync<Country> countryRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _countryRepositoryAsync = countryRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterDepartmentFromNameSpecification(request.Name, null));
            if (checkByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkRegion = await _countryRepositoryAsync.GetByIdAsync(request.CountryId);
            if (checkRegion == null)
            {
                throw new KeyNotFoundException($"No se encontro el pais con el id {request.CountryId}");
            }

            var newRecord = _mapper.Map<Department>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            var dto = _mapper.Map<DepartmentDto>(response);
            return new Response<DepartmentDto>(dto, "Agregado Correctamente");
        }
    }
}
