using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Queries
{
    public class GetUnitOfMeasurementByIdQuery : IRequest<Response<UnitOfMeasurementDto>>
    {
        public int Id { get; set; }
    }

    public class GetUnitOfMeasurementByIdQueryHandler : IRequestHandler<GetUnitOfMeasurementByIdQuery, Response<UnitOfMeasurementDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<UnitOfMeasurement> _repositoryAsync;

        public GetUnitOfMeasurementByIdQueryHandler(IMapper mapper, IRepositoryAsync<UnitOfMeasurement> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<UnitOfMeasurementDto>> Handle(GetUnitOfMeasurementByIdQuery request, CancellationToken cancellationToken)
        {
            var unitOfMeasurement = await _repositoryAsync.GetByIdAsync(request.Id);
            if (unitOfMeasurement == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<UnitOfMeasurementDto>(unitOfMeasurement);
            return new Response<UnitOfMeasurementDto>(dto);
        }
    }
}
