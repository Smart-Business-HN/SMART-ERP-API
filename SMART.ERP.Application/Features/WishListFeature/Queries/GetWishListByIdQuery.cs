using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WishListFeature.Queries
{
    public class GetWishListByIdQuery : IRequest<Response<WishListDto>>
    {
        public int Id { get; set; }
    }

    public class GetWishListByIdQueryHandler : IRequestHandler<GetWishListByIdQuery, Response<WishListDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WishList> _repositoryAsync;

        public GetWishListByIdQueryHandler(IMapper mapper, IRepositoryAsync<WishList> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<WishListDto>> Handle(GetWishListByIdQuery request, CancellationToken cancellationToken)
        {
            var wishList = await _repositoryAsync.FirstOrDefaultAsync(
                new WishListIncludesSpecification(id: request.Id, code: null, customerId: null));
            if (wishList == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<WishListDto>(wishList);
            return new Response<WishListDto>(dto);
        }
    }
}
