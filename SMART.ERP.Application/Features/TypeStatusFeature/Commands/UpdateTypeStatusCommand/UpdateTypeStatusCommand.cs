using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeStatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.UpdateTypeStatusCommand
{
    public class UpdateTypeStatusCommand : IRequest<Response<TypeStatusDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateTypeStatusCommandHandler : IRequestHandler<UpdateTypeStatusCommand, Response<TypeStatusDto>>
    {
        private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateTypeStatusCommandHandler(IRepositoryAsync<TypeStatus> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<TypeStatusDto>> Handle(UpdateTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var checkById = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkById == null)
            {
                throw new KeyNotFoundException($"No se encontro el registro con id {request.Id}");
            }
            var checkName = await _repositoryAsync.FirstOrDefaultAsync(new FilterTypeStatusFromNameSpecification(request.Name, request.Id));
            if (checkName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            checkById.Name = request.Name;
            checkById.IsActive = request.IsActive;

            await _repositoryAsync.UpdateAsync(checkById);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<TypeStatusDto>(checkById);
            return new Response<TypeStatusDto>(dto, "Actualizado correctamente");
        }
    }
}
