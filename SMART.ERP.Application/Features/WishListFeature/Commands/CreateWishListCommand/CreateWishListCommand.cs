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

namespace SMART.ERP.Application.Features.WishListFeature.Commands.CreateWishListCommand
{
    public class CreateWishListCommand : IRequest<Response<WishListDto>>
    {
        public Guid CustomerId { get; set; }
        public List<CreateWishListProductDto> WishListProducts { get; set; } = null!;
    }
    public class CreateWishListCommandHandler : IRequestHandler<CreateWishListCommand, Response<WishListDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<WishList> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _repositoryCustomerAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;

        public CreateWishListCommandHandler(IMapper mapper, IRepositoryAsync<WishList> repositoryAsync, IRepositoryAsync<Customer> repositoryCustomerAsync,
            IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _repositoryCustomerAsync = repositoryCustomerAsync;
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
        }

        public async Task<Response<WishListDto>> Handle(CreateWishListCommand request, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.Now;

            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterWishListFromCountSpecification(request.CustomerId, date));

            if (checkIfExist != null)
            {
                throw new ApiException($"Solo puede agregar una lista de deseos por dia");
            }
            var client = await _repositoryCustomerAsync.GetByIdAsync(request.CustomerId);
            if (client == null)
            {
                throw new ApiException($"No se encontro ningun cliente con el id: ${request.CustomerId}");
            }
            var newRecord = _mapper.Map<WishList>(request);

            var getStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(
                    new FilterStatusFromNameSpecification("Guardado"));
            if (getStatus != null)
                newRecord.StatusId = getStatus!.Id;
            else
                throw new ApiException("Ocurrio un error al asignar un estado a este carrito");

            newRecord.Code = await GenerateCode(date);
            newRecord.CustomerId = client.Id;
            newRecord.CantItems = newRecord.WishListProducts.Count;
            newRecord.IsActive = true;
            newRecord.CreationDate = date;

            foreach (var item in newRecord.WishListProducts)
            {
                var checkProduct = await _productRepositoryAsync.GetByIdAsync(item.ProductId);
                if (checkProduct == null)
                {
                    throw new KeyNotFoundException($"No se encontro el producto con id {item.ProductId}");
                }
                item.StatusId = getStatus!.Id;
                item.CreationDate = date;
                item.IsActive = true;
            }
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WishListDto>(data);
            return new Response<WishListDto>(dto, message: $"Lista de deseo creada exitosamente con código: {dto.Code}");
        }
        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _repositoryAsync.ListAsync(new FilterLastWishListSpecification());
            return CodeIdentity.WLPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}