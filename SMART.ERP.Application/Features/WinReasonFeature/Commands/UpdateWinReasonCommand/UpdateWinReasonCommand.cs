using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.WinReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WinReasonFeature.Commands.UpdateWinReasonCommand
{
    public class UpdateWinReasonCommand : IRequest<Response<WinReasonDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateLossReasonCommandHandler : IRequestHandler<UpdateWinReasonCommand, Response<WinReasonDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WinReason> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateLossReasonCommandHandler(IMapper mapper, IRepositoryAsync<WinReason> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<WinReasonDto>> Handle(UpdateWinReasonCommand request, CancellationToken cancellationToken)
        {
            var winReason = await _repositoryAsync.GetByIdAsync(request.Id);
            if (winReason == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterWinReasonSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            winReason.Name = request.Name;
            winReason.IsActive = request.IsActive;
            winReason.ModificationDate = DateTime.Now;
            winReason.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(winReason);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WinReasonDto>(winReason);
            return new Response<WinReasonDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
