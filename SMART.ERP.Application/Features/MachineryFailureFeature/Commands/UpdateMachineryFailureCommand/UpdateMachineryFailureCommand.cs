using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.UpdateMachineryFailureCommand
{
    public class UpdateMachineryFailureCommand : IRequest<Response<MachineryFailureDto>>
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateMachineryFailureCommandHandler : IRequestHandler<UpdateMachineryFailureCommand, Response<MachineryFailureDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MachineryFailure> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateMachineryFailureCommandHandler(IMapper mapper, IRepositoryAsync<MachineryFailure> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<MachineryFailureDto>> Handle(UpdateMachineryFailureCommand request, CancellationToken cancellationToken)
        {
            var machineryFailure = await _repositoryAsync.GetByIdAsync(request.Id);
            if (machineryFailure == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterMachineryFailureSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            machineryFailure.Name = request.Name;
            machineryFailure.IsActive = request.IsActive;
            machineryFailure.ModificationDate = DateTime.Now;
            machineryFailure.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(machineryFailure);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<MachineryFailureDto>(machineryFailure);
            return new Response<MachineryFailureDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
