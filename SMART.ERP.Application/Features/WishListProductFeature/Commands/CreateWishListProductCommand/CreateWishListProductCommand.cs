using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Specifications.WishListSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.CreateWishListProductCommand
{
    public class CreateWishListProductCommand : IRequest<Response<WishListProductDto>>
    {
        public int ProductId { get; set; }
        public int WishListId { get; set; }
    }

    public class CreateWishListProductCommandHandler : IRequestHandler<CreateWishListProductCommand, Response<WishListProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WishListProduct> _repositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<WishList> _wishListRepositoryAsync;

        public CreateWishListProductCommandHandler(IMapper mapper, IRepositoryAsync<WishListProduct> repositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<WishList> wishListRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _wishListRepositoryAsync = wishListRepositoryAsync;
        }

        public async Task<Response<WishListProductDto>> Handle(CreateWishListProductCommand request, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.Now;
            var newRecord = _mapper.Map<WishListProduct>(request);
            var getStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(
                new FilterStatusFromNameSpecification(StatusName.Status!.Find(x => x == "Guardado")!));
            if (getStatus != null)
                newRecord.StatusId = getStatus!.Id;
            else
                throw new ApiException("Ocurrio un error al asignar un estado a esta Lista de deseo");

            newRecord.IsActive = true;
            newRecord.CreationDate = DateTime.Now;

            var getWishList = await _wishListRepositoryAsync.FirstOrDefaultAsync(
               new WishListIncludesSpecification(id: request.WishListId, code: null, customerId: null));
            if (getWishList == null)
            {
                throw new ApiException($"No se encontro el carrito con el id: {request.WishListId}");
            }
            if (getWishList.WishListProducts != null && getWishList.WishListProducts.Find(x => x.ProductId == request.ProductId) != null)
            {
                throw new ApiException("Este Producto ya existe en tu carrito.");
            }
            newRecord.Quantity = 1;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _wishListRepositoryAsync.SaveChangesAsync();
            await UpdateWishList(request.WishListId);
            var dto = _mapper.Map<WishListProductDto>(data);
            return new Response<WishListProductDto>(dto, message: $"Producto agregado exitosamente");
        }

        public async Task UpdateWishList(int WishListId)
        {
            var getWishList = await _wishListRepositoryAsync.GetByIdAsync(WishListId);
            if (getWishList != null)
            {
                getWishList.CantItems += 1;
                await _wishListRepositoryAsync.UpdateAsync(getWishList);
                await _wishListRepositoryAsync.SaveChangesAsync();
            }
        }
    }
}
