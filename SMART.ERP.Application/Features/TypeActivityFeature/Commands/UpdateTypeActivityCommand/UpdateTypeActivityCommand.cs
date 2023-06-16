using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.TypeActivitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Commands.UpdateTypeActivityCommand
{
    public class UpdateTypeActivityCommand : IRequest<Response<TypeActivityDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateTypeActivityCommandHandler : IRequestHandler<UpdateTypeActivityCommand, Response<TypeActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeActivity> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateTypeActivityCommandHandler(IMapper mapper, IRepositoryAsync<TypeActivity> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<TypeActivityDto>> Handle(UpdateTypeActivityCommand request, CancellationToken cancellationToken)
        {
            var typeActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (typeActivity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterTypeActivitySpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            typeActivity.Name = request.Name;
            typeActivity.ModificationDate = DateTime.Now;
            typeActivity.ModificatedBy = _jwtService.GetSubjectToken();
            typeActivity.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(typeActivity);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TypeActivityDto>(typeActivity);
            return new Response<TypeActivityDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
