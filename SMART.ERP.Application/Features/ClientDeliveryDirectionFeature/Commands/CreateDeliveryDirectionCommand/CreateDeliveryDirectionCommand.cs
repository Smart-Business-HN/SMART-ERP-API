using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.CreateDeliveryDirectionCommand
{
    public class CreateDeliveryDirectionCommand : IRequest<Response<DeliveryDirectionDto>>
    {
        public string Description { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public int CityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class CreateDeliveryDirectionCommandHandler : IRequestHandler<CreateDeliveryDirectionCommand, Response<DeliveryDirectionDto>>
    {
        private readonly IRepositoryAsync<DeliveryDirection> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IMapper _mapper;

        public CreateDeliveryDirectionCommandHandler(IRepositoryAsync<DeliveryDirection> repositoryAsync, IRepositoryAsync<Customer> clientRepositoryHNAsync,
            IRepositoryAsync<City> cityRepositoryHNAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _clientRepositoryAsync = clientRepositoryHNAsync;
            _cityRepositoryAsync = cityRepositoryHNAsync;
            _mapper = mapper;
        }

        public async Task<Response<DeliveryDirectionDto>> Handle(CreateDeliveryDirectionCommand request, CancellationToken cancellationToken)
        {
            var checkIfCustomerExist = await _clientRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (checkIfCustomerExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.CustomerId}");
            }
            var checkIfCityExist = await _cityRepositoryAsync.GetByIdAsync(request.CityId);
            if (checkIfCityExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.CityId}");
            }
            var newRecord = _mapper.Map<DeliveryDirection>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            var dto = _mapper.Map<DeliveryDirectionDto>(response);
            return new Response<DeliveryDirectionDto>(dto, "Dirección de envio creada exitosamente.");
        }
    }
}
