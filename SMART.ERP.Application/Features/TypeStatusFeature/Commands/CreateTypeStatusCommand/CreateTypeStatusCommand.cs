using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeStatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.CreateTypeStatusCommand
{
    public class CreateTypeStatusCommand : IRequest<Response<TypeStatusDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateTypeStatusCommandHandler : IRequestHandler<CreateTypeStatusCommand, Response<TypeStatusDto>>
    {
        private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateTypeStatusCommandHandler(IRepositoryAsync<TypeStatus> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<TypeStatusDto>> Handle(CreateTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var checkByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterTypeStatusFromNameSpecification(request.Name, null));
            if (checkByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            var newRecord = _mapper.Map<TypeStatus>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TypeStatusDto>(response);
            return new Response<TypeStatusDto>(dto, "Agregado correctamente");
        }
    }
}
