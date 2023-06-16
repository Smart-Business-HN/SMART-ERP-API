using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Queries
{
    public class GetTypeActivityByIdQuery : IRequest<Response<TypeActivityDto>>
    {
        public int Id { get; set; }
    }

    public class GetTypeActivityByIdQueryHandler : IRequestHandler<GetTypeActivityByIdQuery, Response<TypeActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<TypeActivity> _repositoryAsync;

        public GetTypeActivityByIdQueryHandler(IMapper mapper, IRepositoryAsync<TypeActivity> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<TypeActivityDto>> Handle(GetTypeActivityByIdQuery request, CancellationToken cancellationToken)
        {
            var typeActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (typeActivity == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<TypeActivityDto>(typeActivity);
            return new Response<TypeActivityDto>(dto);
        }
    }
}
