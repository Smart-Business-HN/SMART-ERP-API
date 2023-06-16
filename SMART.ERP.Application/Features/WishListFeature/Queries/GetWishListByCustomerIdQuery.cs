using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListFeature.Queries
{
    public class GetWishListByCustomerIdQuery : IRequest<Response<WishListDto>>
    {
        public Guid CustomerId { get; set; }

    }

    public class GetWishListByCustomerIdQueryHandler : IRequestHandler<GetWishListByCustomerIdQuery, Response<WishListDto>>
    {
        private readonly IRepositoryAsync<WishList> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetWishListByCustomerIdQueryHandler(IRepositoryAsync<WishList> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<WishListDto>> Handle(GetWishListByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var wishList = await _repositoryAsync.FirstOrDefaultAsync(new WishListIncludesSpecification(null, null, request.CustomerId));
            var dto = _mapper.Map<WishListDto>(wishList);
            return new Response<WishListDto>(dto);
        }
    }
}
