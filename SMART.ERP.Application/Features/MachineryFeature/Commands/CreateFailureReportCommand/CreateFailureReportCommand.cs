using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateFailureReportCommand
{
    public class CreateFailureReportCommand : IRequest<Response<MachineryFailureReportDto>>
    {
        public string Description { get; set; } = null!;
        public int StatusId { get; set; }
        public int MachineryFailureId { get; set; }
        public DateTime CreationDate { get; set; }
        public int MachineryId { get; set; }
    }

    public class CreateFailureReportCommandHandler : IRequestHandler<CreateFailureReportCommand, Response<MachineryFailureReportDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MachineryFailureReport> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateFailureReportCommandHandler(IMapper mapper,
            IRepositoryAsync<MachineryFailureReport> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<MachineryFailureReportDto>> Handle(CreateFailureReportCommand request, CancellationToken cancellationToken)
        {
            if (request.CreationDate.Date > DateTime.UtcNow.Date)
            {
                throw new ApiException($"La fecha no puede ser mayor a la actual");
            }
            else if (request.CreationDate.Date < DateTime.Now.Date)
            {
                var checkIfReport = await _repositoryAsync.FirstOrDefaultAsync(new GetEndFailureReportSpecification(request.MachineryId, request.CreationDate));
                if (checkIfReport != null)
                    throw new ApiException($"Ya existe un reporte de falla para la fecha seleccionada");
            }

            var newRecord = _mapper.Map<MachineryFailureReport>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<MachineryFailureReportDto>(data);
            return new Response<MachineryFailureReportDto>(dto, "Reporte de falla creado exitosamente");
        }
    }
}
