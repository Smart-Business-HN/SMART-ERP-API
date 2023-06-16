using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.WinReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WinReasonFeature.Commands.CreateWinReasonCommand
{
    public class CreateWinReasonCommand : IRequest<Response<WinReasonDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateWinReasonCommandHandler : IRequestHandler<CreateWinReasonCommand, Response<WinReasonDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WinReason> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateWinReasonCommandHandler(IMapper mapper, IRepositoryAsync<WinReason> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<WinReasonDto>> Handle(CreateWinReasonCommand request, CancellationToken cancellationToken)
        {
            var winReason = await _repositoryAsync.FirstOrDefaultAsync(
               new FilterWinReasonSpecification(request.Name, null));
            if (winReason != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<WinReason>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WinReasonDto>(data);
            return new Response<WinReasonDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
