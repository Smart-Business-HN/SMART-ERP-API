using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListProductFeature.Queries
{
    public class GetWishListProductByIdQuery : IRequest<Response<WishListProductDto>>
    {
        public int Id { get; set; }
    }

    public class GetWishListProductByIdQueryHandler : IRequestHandler<GetWishListProductByIdQuery, Response<WishListProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WishListProduct> _repositoryAsync;

        public GetWishListProductByIdQueryHandler(IMapper mapper, IRepositoryAsync<WishListProduct> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<WishListProductDto>> Handle(GetWishListProductByIdQuery request, CancellationToken cancellationToken)
        {
            var wishList = await _repositoryAsync.FirstOrDefaultAsync(
                new WishListProductIncludesSpecification(id: request.Id));
            if (wishList == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<WishListProductDto>(wishList);
            return new Response<WishListProductDto>(dto);
        }
    }
}