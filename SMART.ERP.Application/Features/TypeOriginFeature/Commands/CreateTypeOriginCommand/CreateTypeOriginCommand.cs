using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeOriginSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Commands.CreateTypeOriginCommand
{
    public class CreateTypeOriginCommand : IRequest<Response<TypeOriginDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsProspectOrigin { get; set; }
    }

    public class CreateTypeOriginCommandHandler : IRequestHandler<CreateTypeOriginCommand, Response<TypeOriginDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeOrigin> _repositoryAsync;

        public CreateTypeOriginCommandHandler(IMapper mapper, IRepositoryAsync<TypeOrigin> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<TypeOriginDto>> Handle(CreateTypeOriginCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterTypeOriginSpecification(request.Name, null));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            var newRecord = _mapper.Map<TypeOrigin>(request);
            newRecord.IsActive = request.IsActive;
            newRecord.Name = request.Name;
            newRecord.IsProspectOrigin = request.IsProspectOrigin;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TypeOriginDto>(data);

            return new Response<TypeOriginDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
