using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CaiSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CaiFeature.Queries
{
    public class GetCaiByBranchOfficeIdQuery : IRequest<Response<List<CaiDto>>>
    {
        public int BranchOfficeId {  get; set; } 
    }
    public class GetCaiByBranchOfficeIdQueryHandler : IRequestHandler<GetCaiByBranchOfficeIdQuery, Response<List<CaiDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Cai> _repositoryAsync;
        public GetCaiByBranchOfficeIdQueryHandler(IMapper mapper, IRepositoryAsync<Cai> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<List<CaiDto>>> Handle(GetCaiByBranchOfficeIdQuery request, CancellationToken cancellationToken)
        {

            var cais = await _repositoryAsync.ListAsync(new FilterCaiByBranchOfficeIdSpecification(request.BranchOfficeId));
            var dto = _mapper.Map<List<CaiDto>>(cais);
            return new Response<List<CaiDto>>(dto);
        }
    }
}
