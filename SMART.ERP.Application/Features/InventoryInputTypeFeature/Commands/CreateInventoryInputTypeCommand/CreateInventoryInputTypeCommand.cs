using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.LossReasonFeature.Commands.CreateLossReasonCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryInputTypeSpecification;
using SMART.ERP.Application.Specifications.LossReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.CreateInventoryInputTypeCommand
{
    public class CreateInventoryInputTypeCommand : IRequest<Response<InventoryInputTypeDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateInventoryInputTypeCommandHandler : IRequestHandler<CreateInventoryInputTypeCommand, Response<InventoryInputTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InventoryInputType> _repositoryAsync;

        public CreateInventoryInputTypeCommandHandler(IMapper mapper, IRepositoryAsync<InventoryInputType> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
           
        }
        public async Task<Response<InventoryInputTypeDto>> Handle(CreateInventoryInputTypeCommand request, CancellationToken cancellationToken)
        {
            var inventoryInputType = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterInventoryInputTypeSpecification(request.Name, null));
            if (inventoryInputType != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<InventoryInputType>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InventoryInputTypeDto>(data);
            return new Response<InventoryInputTypeDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
