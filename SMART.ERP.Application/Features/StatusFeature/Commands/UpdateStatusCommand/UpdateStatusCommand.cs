using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.StatusFeature.Commands.UpdateStatusCommand
{
    public class UpdateStatusCommand : IRequest<Response<StatusDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TypeStatusId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, Response<StatusDto>>
    {
        private readonly IRepositoryAsync<Status> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeStatus> _typeStatusRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateStatusCommandHandler(IRepositoryAsync<Status> repositoryAsync, IMapper mapper, IRepositoryAsync<TypeStatus> typeStatusRepositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _typeStatusRepositoryAsync = typeStatusRepositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<StatusDto>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var checkStatus = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el estado con id {request.Id}");
            }
            var checkByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterStatusFromNameExceptIdSpecification(request.Name, request.Id));
            if (checkByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkTypeStatus = await _typeStatusRepositoryAsync.GetByIdAsync(request.TypeStatusId);
            if (checkTypeStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el tipo de estado con id {request.TypeStatusId}");
            }

            checkStatus!.Name = request.Name;
            checkStatus!.IsActive = request.IsActive;
            checkStatus!.TypeStatusId = request.TypeStatusId;

            await _repositoryAsync.UpdateAsync(checkStatus);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_statuses",cancellationToken);
            var dto = _mapper.Map<StatusDto>(checkStatus);

            return new Response<StatusDto>(dto, $"{request.Name} actualizado correctamente");
        }
    }
}
