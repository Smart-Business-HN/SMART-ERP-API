using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BannerFeature.Queries
{
    public class GetAllBannersQuery : IRequest<Response<List<BannerDto>>>
    {
        public class GetAllBannersQueryHandler : IRequestHandler<GetAllBannersQuery, Response<List<BannerDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Banner> _repositoryAsync;

            public GetAllBannersQueryHandler(IMapper mapper, IRepositoryAsync<Banner> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<List<BannerDto>>> Handle(GetAllBannersQuery request, CancellationToken cancellationToken)
            {
                var banners = await _repositoryAsync.ListAsync();
                var dto = _mapper.Map<List<BannerDto>>(banners);
                return new Response<List<BannerDto>>(dto);
            }
        }
    }
}
