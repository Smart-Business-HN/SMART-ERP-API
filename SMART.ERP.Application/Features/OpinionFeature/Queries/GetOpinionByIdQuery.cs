using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.OpinionFeature.Queries
{
    public class GetOpinionByIdQuery : IRequest<Response<OpinionDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpinionByIdQueryHandler : IRequestHandler<GetOpinionByIdQuery, Response<OpinionDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opinion> _repositoryAsync;

        public GetOpinionByIdQueryHandler(IMapper mapper, IRepositoryAsync<Opinion> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<OpinionDto>> Handle(GetOpinionByIdQuery request, CancellationToken cancellationToken)
        {
            var opinion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opinion == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<OpinionDto>(opinion);
            return new Response<OpinionDto>(dto);
        }
    }
}
