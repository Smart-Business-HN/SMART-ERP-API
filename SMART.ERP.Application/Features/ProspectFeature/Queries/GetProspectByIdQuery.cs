using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectFeature.Queries
{
    public class GetProspectByIdQuery : IRequest<Response<ProspectDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetProspectByIdQueryHandler : IRequestHandler<GetProspectByIdQuery, Response<ProspectDto>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetProspectByIdQueryHandler(IRepositoryAsync<Prospect> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ProspectDto>> Handle(GetProspectByIdQuery request, CancellationToken cancellationToken)
        {
            var checkProspect = await _repositoryAsync.FirstOrDefaultAsync(new ProspectIncludesSpecification(request.Id));
            if (checkProspect == null)
            {
                throw new KeyNotFoundException($"No se encontro el prospecto con id {request.Id}");
            }
            var dto = _mapper.Map<ProspectDto>(checkProspect);
            return new Response<ProspectDto>(dto);
        }
    }
}
