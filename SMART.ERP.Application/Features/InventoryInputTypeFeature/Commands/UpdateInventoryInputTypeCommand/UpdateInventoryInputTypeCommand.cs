using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryInputTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.UpdateInventoryInputTypeCommand
{
    public class UpdateInventoryInputTypeCommand : IRequest<Response<InventoryInputTypeDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateInventoryInputTypeCommandHandler : IRequestHandler<UpdateInventoryInputTypeCommand, Response<InventoryInputTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InventoryInputType> _repositoryAsync;

        public UpdateInventoryInputTypeCommandHandler(IMapper mapper, IRepositoryAsync<InventoryInputType> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<InventoryInputTypeDto>> Handle(UpdateInventoryInputTypeCommand request, CancellationToken cancellationToken)
        {
            var inventoryInputType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (inventoryInputType == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterInventoryInputTypeSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            inventoryInputType.Name = request.Name;
            inventoryInputType.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(inventoryInputType);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InventoryInputTypeDto>(inventoryInputType);
            return new Response<InventoryInputTypeDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
