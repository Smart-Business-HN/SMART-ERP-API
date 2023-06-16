using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Queries
{
    public class GetAllUnitOfMeasurementsQuery : IRequest<PagedResponse<List<UnitOfMeasurementDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllUnitOfMeasurementsQueryHandler : IRequestHandler<GetAllUnitOfMeasurementsQuery, PagedResponse<List<UnitOfMeasurementDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<UnitOfMeasurement> _repositoryAsync;

            public GetAllUnitOfMeasurementsQueryHandler(IMapper mapper, IRepositoryAsync<UnitOfMeasurement> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<UnitOfMeasurementDto>>> Handle(GetAllUnitOfMeasurementsQuery request, CancellationToken cancellationToken)
            {

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var unitOfMeasurements = await _repositoryAsync.ListAsync(
                    new QueryUnitOfMeasurementSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<UnitOfMeasurementDto>>(unitOfMeasurements);
                return new PagedResponse<List<UnitOfMeasurementDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
