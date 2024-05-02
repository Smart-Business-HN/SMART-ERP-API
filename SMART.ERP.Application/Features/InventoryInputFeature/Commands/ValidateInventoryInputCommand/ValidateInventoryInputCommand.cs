using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.ValidateInventoryInputCommand
{
    public class ValidateInventoryInputCommand : IRequest<Response<InventoryInputDto>>
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int? PurchaseOrderOriginId { get; set; }
        public int? ProductReturnId { get; set; }
        public int? SurplusInventoryId { get; set; }
        public List<ProductEntryDto> ProductEntries { get; set; }
    }
    public class ValidateInventoryInputCommandHandler : IRequestHandler<ValidateInventoryInputCommand, Response<InventoryInputDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;
        private readonly IRepositoryAsync<InventoryInputType> _inventoryInputTypeRepositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<ProductEntry> _productEntryRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryDistributionRepositoryAsync;
        public ValidateInventoryInputCommandHandler(IMapper mapper, IRepositoryAsync<InventoryInput> repositoryAsync, IRepositoryAsync<InventoryInputType> inventoryInputTypeRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<ProductEntry> productEntryRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryDistributionRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _inventoryInputTypeRepositoryAsync = inventoryInputTypeRepositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _productEntryRepositoryAsync = productEntryRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _inventoryDistributionRepositoryAsync = inventoryDistributionRepositoryAsync;
        }
        public async Task<Response<InventoryInputDto>> Handle(ValidateInventoryInputCommand request, CancellationToken cancellationToken)
        {
            var inventoryInputExist = await _repositoryAsync.GetByIdAsync(request.Id);
            if (inventoryInputExist == null)
            {
                throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");
            }
            var warehouseExist = await _warehouseRepositoryAsync.GetByIdAsync(request.WarehouseId);
            if (warehouseExist == null)
            {
                throw new ApiException($"No existe un almacen con el Id {request.WarehouseId}");
            }
            bool itIsProductsUpdated = await InsertProducts(request.ProductEntries, warehouseExist.Id);
            if (itIsProductsUpdated == false)
            {
                throw new ApiException("Ocurrio un error al actualizar los productos");
            }
            inventoryInputExist.StatusId = 32;

            await _repositoryAsync.UpdateAsync(inventoryInputExist);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InventoryInputDto>(inventoryInputExist);
            return new Response<InventoryInputDto>(dto);
        }
        public async Task<bool> InsertProducts(List<ProductEntryDto> productEntries, int warehouseId)
        {
            foreach (var productEntry in productEntries)
            {
                var warehouseDistributionExist = await _inventoryDistributionRepositoryAsync.FirstOrDefaultAsync(new FilterInventoryDistributionByProductIdAndWarehouseId(productEntry.ProductId, warehouseId));
                if (warehouseDistributionExist == null)
                {
                    var warehouseDistribution = new InventoryDistribution
                    {
                        ProductId = productEntry.ProductId,
                        WarehouseId = warehouseId,
                        Quantity = productEntry.Quantity
                    };
                    await _inventoryDistributionRepositoryAsync.AddAsync(warehouseDistribution);
                    var product = await _productRepositoryAsync.GetByIdAsync(productEntry.ProductId);
                    product.CurrentStock += (int)productEntry.Quantity;
                    await _productRepositoryAsync.UpdateAsync(product);
                }
                else
                {
                    warehouseDistributionExist.Quantity += productEntry.Quantity;
                    var product = warehouseDistributionExist.Product;
                    product.CurrentStock += (int)productEntry.Quantity;
                    await _inventoryDistributionRepositoryAsync.UpdateAsync(warehouseDistributionExist);
                    await _productRepositoryAsync.UpdateAsync(product);
                }
            }
            await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
            await _productRepositoryAsync.SaveChangesAsync();
            return true;
        }
    }
}
