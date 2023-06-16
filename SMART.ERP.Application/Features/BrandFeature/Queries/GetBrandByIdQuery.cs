using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Queries
{
    public class GetBrandByIdQuery : IRequest<Response<BrandDto>>
    {
        public int Id { get; set; }

        public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Response<BrandDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Brand> _repositoryAsync;

            public GetBrandByIdQueryHandler(IMapper mapper, IRepositoryAsync<Brand> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<Response<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
            {
                var brand = await _repositoryAsync.GetByIdAsync(request.Id);
                if (brand == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
                }
                var dto = _mapper.Map<BrandDto>(brand);
                return new Response<BrandDto>(dto);
            }
        }
    }
}
