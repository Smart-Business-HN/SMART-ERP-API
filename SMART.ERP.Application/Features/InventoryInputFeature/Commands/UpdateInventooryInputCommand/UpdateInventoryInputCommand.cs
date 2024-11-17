using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryInputSpecification;
using SMART.ERP.Application.Specifications.ProductEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.UpdateInventooryInputCommand
{
    public class UpdateInventoryInputCommand : IRequest<Response<InventoryInputDto>>
    {
        public int Id { get; set; }
        public int InventoryInputTypeId { get; set; }
        public int WarehouseId { get; set; }
        public string? Description { get; set; }
        public int? PurchaseOrderOriginId { get; set; }
        public int? ProductReturnId { get; set; }
        public int? SurplusInventoryId { get; set; }
        public List<ProductEntryDto>? ProductEntries { get; set; }
        public List<CreateProductEntryDto>? ProductToEntries { get; set; }
    }
    public class UpdateInventoryInputCommandHandler : IRequestHandler<UpdateInventoryInputCommand, Response<InventoryInputDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;
        private readonly IRepositoryAsync<InventoryInputType> _inventoryInputTypeRepositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepositoryAsync;
        private readonly IRepositoryAsync<ProductEntry> _productEntryRepositoryAsync;
        public UpdateInventoryInputCommandHandler(IMapper mapper, IRepositoryAsync<InventoryInput> repositoryAsync, IRepositoryAsync<InventoryInputType> inventoryInputTypeRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<ProductEntry> productEntryRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _inventoryInputTypeRepositoryAsync = inventoryInputTypeRepositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _purchaseOrderRepositoryAsync = purchaseOrderRepositoryAsync;
            _productEntryRepositoryAsync = productEntryRepositoryAsync;
        }
        public async Task<Response<InventoryInputDto>> Handle(UpdateInventoryInputCommand request, CancellationToken cancellationToken)
        {
            var inventoryInputExist = await _repositoryAsync.FirstOrDefaultAsync(new InventoryInputIncludesSpecification(request.Id));
            if (inventoryInputExist == null)
            {
                throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");
            }
            var inventoryInputTypeExist = await _inventoryInputTypeRepositoryAsync.GetByIdAsync(request.InventoryInputTypeId);
            if (inventoryInputTypeExist == null)
            {
                throw new ApiException($"No existe un tipo de entrada de inventario con el Id {request.InventoryInputTypeId}");
            }
            var warehouseExist = await _warehouseRepositoryAsync.GetByIdAsync(request.WarehouseId);
            if (warehouseExist == null)
            {
                throw new ApiException($"No existe un almacen con el Id {request.WarehouseId}");
            }
            if (request.PurchaseOrderOriginId.HasValue)
            {
                var purchaseOrderExist = await _purchaseOrderRepositoryAsync.GetByIdAsync(request.PurchaseOrderOriginId.Value);
                if (purchaseOrderExist == null)
                {
                    throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderOriginId}");
                }
            }
            //add more validations
            //UPDATING VALUES
            if (inventoryInputExist.InventoryInputTypeId != request.InventoryInputTypeId)
            {
                inventoryInputExist.InventoryInputTypeId = request.InventoryInputTypeId;
            }
            if (inventoryInputExist.WarehouseId != request.WarehouseId)
            {
                inventoryInputExist.WarehouseId = request.WarehouseId;
            }
            if (inventoryInputExist.Description != request.Description)
            {
                inventoryInputExist.Description = request.Description;
            }
            if (inventoryInputExist.PurchaseOrderOriginId != request.PurchaseOrderOriginId)
            {
                inventoryInputExist.PurchaseOrderOriginId = request.PurchaseOrderOriginId;
            }
            if (inventoryInputExist.ProductReturnId != request.ProductReturnId)
            {
                inventoryInputExist.ProductReturnId = request.ProductReturnId;
            }
            if (inventoryInputExist.SurplusInventoryId != request.SurplusInventoryId)
            {
                inventoryInputExist.SurplusInventoryId = request.SurplusInventoryId;
            }
            var productEntries = await CheckProducts(request.ProductEntries!, request.ProductToEntries!, inventoryInputExist.Id);
            inventoryInputExist.Prefix = null;
            inventoryInputExist.ProductEntries = null;
            inventoryInputExist.Warehouse = null;
            inventoryInputExist.InventoryInputType = null;
            inventoryInputExist.Status = null;
            await _repositoryAsync.UpdateAsync(inventoryInputExist);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InventoryInputDto>(inventoryInputExist);
            dto.ProductEntries = productEntries;
            return new Response<InventoryInputDto>(dto, $"Entrada {inventoryInputExist.Code} actualizada exitosamente.");
        }
        public async Task<List<ProductEntryDto>> CheckProducts(List<ProductEntryDto> productEntries, List<CreateProductEntryDto> productsToEntry, int inventoryInputId)
        {
            var productsToProcess = new List<CreateProductEntryDto>(productsToEntry);
            var prouductsPreExistence = new List<ProductEntryDto>(productEntries);

            foreach (var productExisting in productEntries)
            {
                //UPDATING EVERY PRODUCT AND REMOVING FROM LOCAL LISTS
                foreach (var item in productsToEntry)
                {
                    if (productExisting.ProductId == item.ProductId)
                    {
                        var productToUpdate = productExisting;
                        if (productToUpdate.Quantity != item.Quantity)
                        {
                            productToUpdate.Quantity = item.Quantity;
                        }
                        if (productToUpdate.UnitProductPrice != item.UnitProductPrice)
                        {
                            productToUpdate.UnitProductPrice = item.UnitProductPrice;
                        }

                        productToUpdate.Total = (productToUpdate.Quantity * productToUpdate.UnitProductPrice);
                        productToUpdate.Product = null;
                        productToUpdate.InventoryInput = null;
                        var productSeed = _mapper.Map<ProductEntry>(productToUpdate);
                        await _productEntryRepositoryAsync.UpdateAsync(productSeed);
                        await _productEntryRepositoryAsync.SaveChangesAsync();
                        prouductsPreExistence.RemoveAll(x => x.ProductId == productExisting.ProductId);
                        productsToProcess.RemoveAll(x => x.ProductId == item.ProductId);
                    }
                }
            }
            //ADD NEW PRODUCTS TO THE Inventory Entry
            foreach (var newProductToEntry in productsToProcess)
            {
                var newProductEntry = _mapper.Map<ProductEntry>(newProductToEntry);
                newProductEntry.InventoryInputId = inventoryInputId;
                newProductEntry.UnitProductPrice = newProductToEntry.UnitProductPrice;
                newProductEntry.Total = newProductToEntry.Quantity * newProductToEntry.UnitProductPrice;
                await _productEntryRepositoryAsync.AddAsync(newProductEntry);
                await _productEntryRepositoryAsync.SaveChangesAsync();
            }
            //REMOVING PREVIOUS PRODUCTS FROM THE INVOICE
            foreach (var productPreExistence in prouductsPreExistence)
            {
                productPreExistence.InventoryInput = null;
                productPreExistence.Product = null;

                var dtoToDelete = _mapper.Map<ProductEntry>(productPreExistence);
                await _productEntryRepositoryAsync.DeleteAsync(dtoToDelete);
                await _productEntryRepositoryAsync.SaveChangesAsync();
            }
            //GETS NEW PRODUCTS FROM THE QUOTATION
            var newProducts = await _productEntryRepositoryAsync.ListAsync(new ProductEntrySpecification(inventoryInputId));
            var dtos = _mapper.Map<List<ProductEntryDto>>(newProducts);

            return dtos;
        }
    }

}
