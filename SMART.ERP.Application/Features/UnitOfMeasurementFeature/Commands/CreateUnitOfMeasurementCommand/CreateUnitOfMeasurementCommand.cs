using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.CreateUnitOfMeasurementCommand
{
    public class CreateUnitOfMeasurementCommand : IRequest<Response<UnitOfMeasurementDto>>
    {
        public string Name { get; set; } = null!;
        public string Abreviation { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateUnitOfMeasurementCommandHandler : IRequestHandler<CreateUnitOfMeasurementCommand, Response<UnitOfMeasurementDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<UnitOfMeasurement> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateUnitOfMeasurementCommandHandler(IMapper mapper, IRepositoryAsync<UnitOfMeasurement> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<UnitOfMeasurementDto>> Handle(CreateUnitOfMeasurementCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUnitOfMeasurementSpecification(request.Name, null));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkIfExistByAbreviation = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUnitOfMeasurementSpecification(request.Name, null));
            if (checkIfExistByAbreviation != null)
            {
                throw new ApiException($"Ya existe un registro con la abreviacion {request.Abreviation}");
            }

            var newRecord = _mapper.Map<UnitOfMeasurement>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<UnitOfMeasurementDto>(data);

            return new Response<UnitOfMeasurementDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
