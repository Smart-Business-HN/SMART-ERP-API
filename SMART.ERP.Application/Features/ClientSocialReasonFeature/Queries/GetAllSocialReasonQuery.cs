using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ClientSocialReasonFeature.Queries
{
    public class GetAllSocialReasonQuery : IRequest<Response<List<SocialReasonDto>>>
    {
        public class GetAllSocialReasonQueryHandler : IRequestHandler<GetAllSocialReasonQuery, Response<List<SocialReasonDto>>>
        {
            private readonly IRepositoryAsync<SocialReason> _repositoryHNAsync;
            private readonly IMapper _mapper;

            public GetAllSocialReasonQueryHandler(IRepositoryAsync<SocialReason> repositoryHNAsync, IMapper mapper)
            {
                _repositoryHNAsync = repositoryHNAsync;
                _mapper = mapper;
            }
            public async Task<Response<List<SocialReasonDto>>> Handle(GetAllSocialReasonQuery request, CancellationToken cancellationToken)
            {
                var socialReasonList = await _repositoryHNAsync.ListAsync();
                var dto = _mapper.Map<List<SocialReasonDto>>(socialReasonList);
                return new Response<List<SocialReasonDto>>(dto);
            }
        }
    }
}
