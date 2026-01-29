using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BannerFeature.Queries
{
    public class GetBannerByIdQuery : IRequest<Response<BannerDto>>
    {
        public int Id { get; set; }
    }

    public class GetBannerByIdQueryHandler : IRequestHandler<GetBannerByIdQuery, Response<BannerDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Banner> _repositoryAsync;

        public GetBannerByIdQueryHandler(IMapper mapper, IRepositoryAsync<Banner> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<BannerDto>> Handle(GetBannerByIdQuery request, CancellationToken cancellationToken)
        {
            var banner = await _repositoryAsync.GetByIdAsync(request.Id);
            if (banner == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<BannerDto>(banner);
            return new Response<BannerDto>(dto);
        }
    }
}
