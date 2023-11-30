using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.StatusFeature.Commands.CreateStatusCommand
{
    public class CreateStatusCommand : IRequest<Response<StatusDto>>
    {
        public string Name { get; set; } = null!;
        public int TypeStatusId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStatusCommandHandler : IRequestHandler<CreateStatusCommand, Response<StatusDto>>
    {
        private readonly IRepositoryAsync<Status> _repositoryAsync;
        private readonly IRepositoryAsync<TypeStatus> _typeStatusRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;
        public CreateStatusCommandHandler(IRepositoryAsync<Status> repositoryAsync, IRepositoryAsync<TypeStatus> typeStatusRepositoryAsync
            , IMapper mapper, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _typeStatusRepositoryAsync = typeStatusRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<StatusDto>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
        {
            var checkByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameSpecification(request.Name));
            if (checkByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkTypeStatus = await _typeStatusRepositoryAsync.GetByIdAsync(request.TypeStatusId);
            if (checkTypeStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el tipo de estado con id {request.TypeStatusId}");
            }

            var newRecord = _mapper.Map<Status>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_statuses", cancellationToken);
            var dto = _mapper.Map<StatusDto>(response);
            return new Response<StatusDto>(dto, "Agregado correctamente");
        }
    }
}
