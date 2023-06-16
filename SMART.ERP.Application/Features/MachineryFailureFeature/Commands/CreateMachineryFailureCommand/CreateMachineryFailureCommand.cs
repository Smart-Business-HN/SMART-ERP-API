using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.CreateMachineryFailureCommand
{
    public class CreateMachineryFailureCommand : IRequest<Response<MachineryFailureDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateMachineryFailureCommandHandler : IRequestHandler<CreateMachineryFailureCommand, Response<MachineryFailureDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MachineryFailure> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateMachineryFailureCommandHandler(IMapper mapper,
            IRepositoryAsync<MachineryFailure> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<MachineryFailureDto>> Handle(CreateMachineryFailureCommand request, CancellationToken cancellationToken)
        {
            var machineryFailure = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterMachineryFailureSpecification(request.Name, null));
            if (machineryFailure != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<MachineryFailure>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<MachineryFailureDto>(data);
            return new Response<MachineryFailureDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
