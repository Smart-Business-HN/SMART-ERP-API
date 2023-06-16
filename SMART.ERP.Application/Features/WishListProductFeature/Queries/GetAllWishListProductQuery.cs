using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListProductFeature.Queries
{
    public class GetAllWishListProductQuery : IRequest<Response<List<WishListProductDto>>>
    {
        public class GetAllWishListProductQueryHandler : IRequestHandler<GetAllWishListProductQuery, Response<List<WishListProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<WishListProduct> _repositoryAsync;

            public GetAllWishListProductQueryHandler(IMapper mapper, IRepositoryAsync<WishListProduct> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<WishListProductDto>>> Handle(GetAllWishListProductQuery request, CancellationToken cancellationToken)
            {
                var wishListProducts = await _repositoryAsync.ListAsync(
                    new WishListProductIncludesSpecification(id: null));
                var dto = _mapper.Map<List<WishListProductDto>>(wishListProducts);
                return new Response<List<WishListProductDto>>(dto);
            }
        }
    }
}
