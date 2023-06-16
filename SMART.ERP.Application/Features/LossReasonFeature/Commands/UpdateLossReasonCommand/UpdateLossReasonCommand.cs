using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.LossReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.UpdateLossReasonCommand
{
    public class UpdateLossReasonCommand : IRequest<Response<LossReasonDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateLossReasonCommandHandler : IRequestHandler<UpdateLossReasonCommand, Response<LossReasonDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<LossReason> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateLossReasonCommandHandler(IMapper mapper, IRepositoryAsync<LossReason> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<LossReasonDto>> Handle(UpdateLossReasonCommand request, CancellationToken cancellationToken)
        {
            var lossReason = await _repositoryAsync.GetByIdAsync(request.Id);
            if (lossReason == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterLossReasonSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            lossReason.Name = request.Name;
            lossReason.IsActive = request.IsActive;
            lossReason.ModificationDate = DateTime.Now;
            lossReason.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(lossReason);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<LossReasonDto>(lossReason);
            return new Response<LossReasonDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
