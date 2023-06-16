using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.StatusFeature.Queries
{
    public class GetStatusByIdQuery : IRequest<Response<StatusDto>>
    {
        public int Id { get; set; }
    }

    public class GetStatusByIdQueryHandler : IRequestHandler<GetStatusByIdQuery, Response<StatusDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Status> _repositoryAsync;

        public GetStatusByIdQueryHandler(IMapper mapper, IRepositoryAsync<Status> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<StatusDto>> Handle(GetStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var status = await _repositoryAsync.GetByIdAsync(request.Id);
            if (status == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<StatusDto>(status);
            return new Response<StatusDto>(dto);
        }
    }
}
