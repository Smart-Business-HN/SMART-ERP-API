using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

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
        private readonly IRepositoryHNAsync<DeliveryDirection> _repositoryHNAsync;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryHNAsync;
        private readonly IRepositoryHNAsync<ClientCity> _cityRepositoryHNAsync;
        private readonly IMapper _mapper;

        public CreateDeliveryDirectionCommandHandler(IRepositoryHNAsync<DeliveryDirection> repositoryHNAsync, IRepositoryHNAsync<Client> clientRepositoryHNAsync,
            IRepositoryHNAsync<ClientCity> cityRepositoryHNAsync, IMapper mapper)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _clientRepositoryHNAsync = clientRepositoryHNAsync;
            _cityRepositoryHNAsync = cityRepositoryHNAsync;
            _mapper = mapper;
        }

        public async Task<Response<DeliveryDirectionDto>> Handle(CreateDeliveryDirectionCommand request, CancellationToken cancellationToken)
        {
            var checkIfCustomerExist = await _clientRepositoryHNAsync.GetByIdAsync(request.CustomerId);
            if (checkIfCustomerExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.CustomerId}");
            }
            var checkIfCityExist = await _cityRepositoryHNAsync.GetByIdAsync(request.CityId);
            if (checkIfCityExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.CityId}");
            }
            var newRecord = _mapper.Map<DeliveryDirection>(request);
            var response = await _repositoryHNAsync.AddAsync(newRecord);
            var dto = _mapper.Map<DeliveryDirectionDto>(response);
            return new Response<DeliveryDirectionDto>(dto, "Dirección de envio creada exitosamente.");
        }
    }
}
