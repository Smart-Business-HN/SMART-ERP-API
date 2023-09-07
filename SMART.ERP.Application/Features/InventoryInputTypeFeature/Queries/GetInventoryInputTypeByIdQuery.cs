using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.Features.MessageFeature.Queries;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Queries
{
    public class GetInventoryInputTypeByIdQuery : IRequest<Response<InventoryInputTypeDto>>
    {
        public int Id { get; set; }
    }

    public class GetInventoryInputTypeByIdQueryHandler : IRequestHandler<GetInventoryInputTypeByIdQuery, Response<InventoryInputTypeDto>>
    {
        private readonly IRepositoryAsync<InventoryInputType> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetInventoryInputTypeByIdQueryHandler(IRepositoryAsync<InventoryInputType> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<InventoryInputTypeDto>> Handle(GetInventoryInputTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var inventoryInputType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (inventoryInputType == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<InventoryInputTypeDto>(inventoryInputType);
            return new Response<InventoryInputTypeDto>(dto);
        }
    }
}
