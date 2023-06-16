using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeOriginSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Commands.UpdateTypeOriginCommand
{
    public class UpdateTypeOriginCommand : IRequest<Response<TypeOriginDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsProspectOrigin { get; set; }
    }

    public class UpdateTypeOriginCommandHandler : IRequestHandler<UpdateTypeOriginCommand, Response<TypeOriginDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeOrigin> _repositoryAsync;

        public UpdateTypeOriginCommandHandler(IMapper mapper, IRepositoryAsync<TypeOrigin> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<TypeOriginDto>> Handle(UpdateTypeOriginCommand request, CancellationToken cancellationToken)
        {
            var typeOrigin = await _repositoryAsync.GetByIdAsync(request.Id);
            if (typeOrigin == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterTypeOriginSpecification(request.Name, request.Id));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            typeOrigin.Name = request.Name;
            typeOrigin.IsActive = request.IsActive;
            typeOrigin.IsProspectOrigin = request.IsProspectOrigin;
            await _repositoryAsync.UpdateAsync(typeOrigin);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TypeOriginDto>(typeOrigin);
            return new Response<TypeOriginDto>(dto, message: $"{typeOrigin.Name} actualizado correctamente");
        }
    }
}
