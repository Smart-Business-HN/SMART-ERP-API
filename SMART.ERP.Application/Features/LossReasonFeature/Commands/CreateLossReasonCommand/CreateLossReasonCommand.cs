using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.LossReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.CreateLossReasonCommand
{
    public class CreateLossReasonCommand : IRequest<Response<LossReasonDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateLossReasonCommandHandler : IRequestHandler<CreateLossReasonCommand, Response<LossReasonDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<LossReason> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateLossReasonCommandHandler(IMapper mapper, IRepositoryAsync<LossReason> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<LossReasonDto>> Handle(CreateLossReasonCommand request, CancellationToken cancellationToken)
        {
            var lossReason = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterLossReasonSpecification(request.Name, null));
            if (lossReason != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<LossReason>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<LossReasonDto>(data);
            return new Response<LossReasonDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
