using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.BranchOfficeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.CreateBranchOfficeCommand
{
    public class CreateBranchOfficeCommand : IRequest<Response<BranchOfficeDto>>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public float Lat { get; set; }
        public float Lng { get; set; }
        public bool IsActive { get; set; }
        public bool IsMainBranchOffice { get; set; }
    }

    public class CreateBranchOfficeCommandHandler : IRequestHandler<CreateBranchOfficeCommand, Response<BranchOfficeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;
        private readonly IRepositoryAsync<Company> _companyRepositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateBranchOfficeCommandHandler(IMapper mapper, IRepositoryAsync<BranchOffices> repositoryAsync,
            IJwtService jwtService, IRepositoryAsync<Company> companyRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _companyRepositoryAsync = companyRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<BranchOfficeDto>> Handle(CreateBranchOfficeCommand request, CancellationToken cancellationToken)
        {
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterBranchOfficeSpecification(request.Name, null, false));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una sucursal con el nombre {request.Name}");
            }

            if (request.IsMainBranchOffice)
            {
                var checkIfExistMain = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterBranchOfficeSpecification(request.Name, null, true));
                if (checkIfExistMain != null)
                {
                    throw new ApiException($"Ya existe una sucursal principal");
                }
            }

            var newRecord = _mapper.Map<BranchOffices>(request);
            var company = await _companyRepositoryAsync.ListAsync();
            if (company.Count() == 0)
                throw new ApiException("Debe registrar los datos de la empresa");
            newRecord.CompanyId = company[0].Id;
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<BranchOfficeDto>(data);
            return new Response<BranchOfficeDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
