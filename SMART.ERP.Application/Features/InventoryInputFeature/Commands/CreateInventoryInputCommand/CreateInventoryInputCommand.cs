using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommand
{
    public class CreateInventoryInputCommand : IRequest<Response<InventoryInputDto>>
    {
        public int InventoryInputTypeId { get; set; }
        public int WarehouseId { get; set; }
        public int PrefixId { get; set; }
        public string? Description { get; set; }
        public int? PurchaseOrderOriginId { get; set; }
        public int? ProductReturnId { get; set; }
        public int? SurplusInventoryId { get; set; }
        public List<CreateProductEntryDto>? ProductEntries { get; set; }
    }
    public class CreateInventoyInputCommandHandler : IRequestHandler<CreateInventoryInputCommand,Response<InventoryInputDto>>
    {
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<Prefix> _prefixRepositoryAsync;
        private readonly IRepositoryAsync<InventoryInputType> _inventoryInputTypeRepositoryAsync;
        private readonly IRepositoryAsync<ProductEntry> _productEntryRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        public CreateInventoyInputCommandHandler(IRepositoryAsync<InventoryInput> repositoryAsync,IJwtService jwtService,IMapper mapper,IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<InventoryInputType> inventoryInputTypeRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<Prefix> prefixRepositoryAsync, IRepositoryAsync<ProductEntry> productEntoryRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _prefixRepositoryAsync = prefixRepositoryAsync;
            _inventoryInputTypeRepositoryAsync = inventoryInputTypeRepositoryAsync;
            _productEntryRepositoryAsync = productEntoryRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<Response<InventoryInputDto>> Handle(CreateInventoryInputCommand request, CancellationToken cancellationToken)
        {
            var prefixExist = await _prefixRepositoryAsync.GetByIdAsync(request.PrefixId);
            if (prefixExist == null)
            {
                throw new ApiException($"No existe un prefijo con el id {request.PrefixId}");
            }
            var warehouseExist = await _warehouseRepositoryAsync.GetByIdAsync(request.WarehouseId);
            if (warehouseExist == null)
            {
                throw new ApiException($"No existe un almacen con el id {request.WarehouseId}");
            }
            var inventoryInputTypeExist = await _inventoryInputTypeRepositoryAsync.GetByIdAsync(request.WarehouseId);
            if (inventoryInputTypeExist == null)
            {
                throw new ApiException($"No existe un Tipo de Entrada de inventario con el id {request.InventoryInputTypeId}");
            }
            if (request.ProductEntries != null)
            {
                var checkProducts = await CheckMasterDataOfProductToInput(request.ProductEntries);
                if (checkProducts != "true")
                {
                    throw new ApiException($"{checkProducts}");
                }
            } 
            var currentInventoryInputs = await _repositoryAsync.ListAsync();
            var productEntries = new List<ProductEntryDto>();
            var newRecord = _mapper.Map<InventoryInput>(request);
            newRecord.Code = CreateInventoryEntryCode(prefixExist, currentInventoryInputs.Last());
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.ProductEntries = null;
            var inventoryEntryResponse = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            //TODO: Create the function to update the products prices by Warehouse.
            if(request.ProductEntries != null && request.ProductEntries.Count > 0)
            {
                foreach (var item in request.ProductEntries)
                {
                    var newProductEntry = _mapper.Map<ProductEntry>(item);
                    newProductEntry.InventoryInputId = inventoryEntryResponse.Id;
                    var productEntryResponse = await _productEntryRepositoryAsync.AddAsync(newProductEntry);
                    await _productEntryRepositoryAsync.SaveChangesAsync();
                    var newProductEntryDto = _mapper.Map<ProductEntryDto>(productEntryResponse);
                    productEntries.Add(newProductEntryDto);
                }
            }
            var dto = _mapper.Map<InventoryInputDto>(inventoryEntryResponse);
            dto.ProductEntries = productEntries;
            return new Response<InventoryInputDto>(dto);
        }
        public async Task<string> CheckMasterDataOfProductToInput (List<CreateProductEntryDto> productEntries)
        {
            foreach (var product in productEntries)
            {
                var productExist = await _productRepositoryAsync.GetByIdAsync(product.ProductId);
                if (productExist == null)
                {
                    return "Ha habido un problema con el Id de uno de los productos";
                }
            }
            return "true";
        }
        public static string CreateInventoryEntryCode(Prefix prefix, InventoryInput lastInventoryInput)
        {
            int numberOfCharacters = prefix.Format.ToCharArray().Length;
            int numberOfCharactersInId = lastInventoryInput.Id.ToString().ToCharArray().Length;
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
