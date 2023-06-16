using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.TypeActivitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Commands.CreateTypeActivityCommand
{
    public class CreateTypeActivityCommand : IRequest<Response<TypeActivityDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateTypeActivityCommandHandler : IRequestHandler<CreateTypeActivityCommand, Response<TypeActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeActivity> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateTypeActivityCommandHandler(IMapper mapper, IRepositoryAsync<TypeActivity> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<TypeActivityDto>> Handle(CreateTypeActivityCommand request, CancellationToken cancellationToken)
        {
            var typeActivity = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterTypeActivitySpecification(request.Name, null));
            if (typeActivity != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<TypeActivity>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TypeActivityDto>(data);
            return new Response<TypeActivityDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
