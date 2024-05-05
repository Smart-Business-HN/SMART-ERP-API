using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommandByPurchaseOrderIdCommand
{
    public class CreateInventoryInputByPurchaseOrderIdCommand : IRequest<Response<int>>
    {
        public int WarehouseId { get; set; }
        public int PrefixId { get; set; }
        public int PurchaseOrderId { get; set; }
        public List<ProductToBuyDto>? ProductEntries { get; set; }
    }
    public class CreateInventoryInputByPurchaseOrderIdCommandHandler : IRequestHandler<CreateInventoryInputByPurchaseOrderIdCommand, Response<int>>
    {
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<ProductEntry> _productEntryRepositoryAsync;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRespositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        public CreateInventoryInputByPurchaseOrderIdCommandHandler(IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<InventoryInput> repositoryAsync, IJwtService jwtService, IMapper mapper, IRepositoryAsync<PurchaseOrder> purchaseOrderRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<ProductEntry> productEntoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _productEntryRepositoryAsync = productEntoryRepositoryAsync;
            _purchaseOrderRespositoryAsync = purchaseOrderRepositoryAsync;
            _mapper = mapper;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<int>> Handle(CreateInventoryInputByPurchaseOrderIdCommand request, CancellationToken cancellationToken)
        {
            var warehouseExist = await _warehouseRepositoryAsync.GetByIdAsync(request.WarehouseId);
            if (warehouseExist == null)
            {
                throw new ApiException($"No existe un almacen con el id {request.WarehouseId}");
            }
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var purchaseOrderExist = await _purchaseOrderRespositoryAsync.GetByIdAsync(request.PurchaseOrderId);
            if (purchaseOrderExist == null)
            {
                throw new ApiException($"No existe una orden de compra con el id {request.PurchaseOrderId}");
            }
            var productEntries = new List<ProductEntryDto>();
            var currentInventoryInputs = await _repositoryAsync.ListAsync();
            var newRecord = new InventoryInput();
            newRecord.WarehouseId = request.WarehouseId;
            newRecord.PrefixId = request.PrefixId;
            newRecord.PurchaseOrderOriginId = request.PurchaseOrderId;
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.Code = CreateInventoryEntryCode(prefixExist, currentInventoryInputs.Last());
            newRecord.ProductEntries = null;
            newRecord.StatusId = 31;
            newRecord.InventoryInputTypeId = 1;
            var inventoryEntryResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            if (request.ProductEntries != null && request.ProductEntries.Count > 0)
            {
                foreach (var item in request.ProductEntries)
                {
                    var newProductEntry = _mapper.Map<ProductEntry>(item);
                    newProductEntry.InventoryInputId = inventoryEntryResponse.Id;
                    newProductEntry.UnitProductPrice = item.UnitPrice;
                    newProductEntry.Quantity = item.Quantity;
                    newProductEntry.Total = item.TotalLine;
                    var productEntryResponse = await _productEntryRepositoryAsync.AddAsync(newProductEntry);

                    var newProductEntryDto = _mapper.Map<ProductEntryDto>(productEntryResponse);
                    productEntries.Add(newProductEntryDto);
                }
                await _productEntryRepositoryAsync.SaveChangesAsync();
            }
            if (purchaseOrderExist.PurchaseBillDestinationId != null)
            {
                purchaseOrderExist.StatusId = 24;
            }
            else
            {
                purchaseOrderExist.StatusId = 23;
            }
            purchaseOrderExist.InventoryInputDestinationId = inventoryEntryResponse.Id;
            await _purchaseOrderRespositoryAsync.UpdateAsync(purchaseOrderExist);
            await _purchaseOrderRespositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InventoryInputDto>(inventoryEntryResponse);
            return new Response<int>(dto.Id);
        }
        public static string CreateInventoryEntryCode(Prefix prefix, InventoryInput lastInventoryInput)
        {
            int numberOfCharacters = prefix.Format.ToCharArray().Length;
            int numberOfCharactersInId = (lastInventoryInput.Id + 1).ToString().ToCharArray().Length;
            string? code;
            if (numberOfCharacters + numberOfCharactersInId < 8)
            {
                int characterOffset = 8 - (numberOfCharacters + numberOfCharactersInId);
                code = prefix.Format + new string('0', characterOffset) + (lastInventoryInput.Id + 1).ToString();
            }
            else
            {
                code = prefix.Format + (lastInventoryInput.Id + 1).ToString();
            }
            return code;
        }

    }

}
