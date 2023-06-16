using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.UpdateUnitOfMeasurementCommand
{
    public class UpdateUnitOfMeasurementCommand : IRequest<Response<UnitOfMeasurementDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Abreviation { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateUnitOfMeasurementCommandHandler : IRequestHandler<UpdateUnitOfMeasurementCommand, Response<UnitOfMeasurementDto>>
    {
        private readonly IRepositoryAsync<UnitOfMeasurement> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateUnitOfMeasurementCommandHandler(IMapper mapper, IRepositoryAsync<UnitOfMeasurement> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<UnitOfMeasurementDto>> Handle(UpdateUnitOfMeasurementCommand request, CancellationToken cancellationToken)
        {
            var unitOfMeasurement = await _repositoryAsync.GetByIdAsync(request.Id);
            if (unitOfMeasurement == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterUnitOfMeasurementSpecification(request.Name, request.Id));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkIfExistByAbreviation = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterUnitOfMeasurementSpecification(request.Name, request.Id));
            if (checkIfExistByAbreviation != null)
            {
                throw new ApiException($"Ya existe un registro con la abreviacion {request.Abreviation}");
            }

            unitOfMeasurement.Name = request.Name;
            unitOfMeasurement.Abreviation = request.Abreviation;
            unitOfMeasurement.IsActive = request.IsActive;
            unitOfMeasurement.ModificatedBy = _jwtService.GetSubjectToken();
            unitOfMeasurement.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(unitOfMeasurement);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<UnitOfMeasurementDto>(unitOfMeasurement);
            return new Response<UnitOfMeasurementDto>(dto, message: $"{unitOfMeasurement.Name} actualizado correctamente");
        }
    }
}
