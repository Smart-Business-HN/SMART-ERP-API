using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TypeStatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Queries
{
    public class GetTypeStatusByIdQuery : IRequest<Response<TypeStatusDto>>
    {
        public int Id { get; set; }
    }

    public class GetTypeStatusByIdQueryHandler : IRequestHandler<GetTypeStatusByIdQuery, Response<TypeStatusDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;

        public GetTypeStatusByIdQueryHandler(IMapper mapper, IRepositoryAsync<TypeStatus> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<TypeStatusDto>> Handle(GetTypeStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var typeStatus = await _repositoryAsync.FirstOrDefaultAsync(new TypeStatusIncludesSpecification(id: null));
            if (typeStatus == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<TypeStatusDto>(typeStatus);
            return new Response<TypeStatusDto>(dto);
        }
    }
}
