using AutoMapper;
using MediatR;
        
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.UpdateDeliveryDirectionCommand
{
    public class UpdateDeliveryDirectionCommand : IRequest<Response<DeliveryDirectionDto>>
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public int CityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class UpdateDeliveryDirectionCommandHandler : IRequestHandler<UpdateDeliveryDirectionCommand, Response<DeliveryDirectionDto>>
    {
        private readonly IRepositoryAsync<DeliveryDirection> _repositoryHNAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryHNAsync;
        private readonly IMapper _mapper;

        public UpdateDeliveryDirectionCommandHandler(IRepositoryAsync<DeliveryDirection> repositoryHNAsync, IRepositoryAsync<City> cityRepositoryHNAsync, IMapper mapper)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _cityRepositoryHNAsync = cityRepositoryHNAsync;
            _mapper = mapper;
        }
        public async Task<Response<DeliveryDirectionDto>> Handle(UpdateDeliveryDirectionCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryHNAsync.GetByIdAsync(request.Id);
            if (checkIfExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la direccion con id {request.Id}");
            }
            var checkIfCityExist = await _cityRepositoryHNAsync.GetByIdAsync(request.CityId);
            if (checkIfCityExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.CityId}");
            }
            checkIfExist.Id = request.Id;
            checkIfExist.Description = request.Description;
            checkIfExist.PhoneNumber = request.PhoneNumber;
            checkIfExist.AdditionalInformation = request.AdditionalInformation;
            checkIfExist.PostalCode = request.PostalCode;
            checkIfExist.CityId = request.CityId;
            checkIfExist.IsActive = request.IsActive;
            await _repositoryHNAsync.UpdateAsync(checkIfExist);
            await _repositoryHNAsync.SaveChangesAsync();
            var dto = _mapper.Map<DeliveryDirectionDto>(checkIfExist);
            return new Response<DeliveryDirectionDto>(dto, "Dirección de envio actualizada exitosamente.");
        }
    }
}
