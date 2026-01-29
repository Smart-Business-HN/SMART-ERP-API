using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.CancelInvoiceCommand
{
    public class CancelInvoiceCommand : IRequest<Response<InvoiceDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductSoldDto>? ProductsSold { get; set; }
        public int StatusId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
    }
    public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, Response<InvoiceDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryDistributionRepositoryAsync;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        public CancelInvoiceCommandHandler(IRepositoryAsync<Invoice> invoiceRepository, IMapper mapper, IJwtService jwtService, IRepositoryAsync<InventoryDistribution> inventoryDistributionRepositoryAsync, IRepositoryAsync<Warehouse> warehouseRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _inventoryDistributionRepositoryAsync = inventoryDistributionRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
        }
        public async Task<Response<InvoiceDto>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _invoiceRepository.GetByIdAsync(request.Id);
            if (invoiceExist == null)
            {
                throw new ApiException($"No existe una factura con el Id {request.CustomerId}");
            }
            invoiceExist.StatusId = 17;
            invoiceExist.ModificationDate = DateTime.UtcNow;
            invoiceExist.ModificatedBy = _jwtService.GetSubjectToken();

            string localProductSoldJson = JsonConvert.SerializeObject(request.ProductsSold);
            var productsPreExistencea = JsonConvert.DeserializeObject<List<ProductSoldDto>>(localProductSoldJson);

            string localProductSoldJson1 = JsonConvert.SerializeObject(request.ProductsSold);
            var productsPreExistencea1 = JsonConvert.DeserializeObject<List<ProductSoldDto>>(localProductSoldJson);

            foreach (var item in productsPreExistencea!)
            {
                item.Product = null;
            }
            await _invoiceRepository.UpdateAsync(invoiceExist);
            await _invoiceRepository.SaveChangesAsync();
            await UpdateStock(productsPreExistencea, invoiceExist.BranchOfficeId);
            await UpdateMainStock(productsPreExistencea1!);
            request.StatusId = 17;
            var invoiceDto = _mapper.Map<InvoiceDto>(request);
            return new Response<InvoiceDto>(invoiceDto, "Factura Cancelada Existosamente");
        }
        public async Task UpdateMainStock(List<ProductSoldDto> productsSold)
        {
            var products = await _productRepositoryAsync.ListAsync();
            var productsPreExistence = new List<ProductSoldDto>(productsSold);

            //Devolver Stock de productos eliminados de la factura
            foreach (var productPreExistence in productsPreExistence)
            {
                var currentStock = products.FirstOrDefault(p => p.Id == productPreExistence.ProductId);
                if (currentStock == null)
                {
                    continue;
                }
                else
                {
                    currentStock.CurrentStock += (int)productPreExistence.Quantity;
                    await _productRepositoryAsync.UpdateAsync(currentStock);
                }
            }
            await _productRepositoryAsync.SaveChangesAsync();
        }


        public async Task UpdateStock(List<ProductSoldDto> productsSold, int branchOfficeId)
        {
            var warehouse = await _warehouseRepositoryAsync.FirstOrDefaultAsync(new FilterWarehouseByBranchOfficeIdSpecification(branchOfficeId));
            if (warehouse == null) { return; }
            var productsPreExistence = new List<ProductSoldDto>(productsSold);
            //Devolver Stock de productos eliminados de la factura
            foreach (var productPreExistence in productsPreExistence)
            {
                var currentStock = warehouse!.InventoryDistributions!.FirstOrDefault(p => p.ProductId == productPreExistence.ProductId);
                if (currentStock == null)
                {
                    var newInventoryDistribution = new InventoryDistribution();
                    newInventoryDistribution.WarehouseId = warehouse.Id;
                    newInventoryDistribution.ProductId = productPreExistence.ProductId!.Value;
                    newInventoryDistribution.Quantity = productPreExistence.Quantity;
                    await _inventoryDistributionRepositoryAsync.AddAsync(newInventoryDistribution);
                }
                else
                {
                    currentStock.Quantity += productPreExistence.Quantity;
                    await _inventoryDistributionRepositoryAsync.UpdateAsync(currentStock);
                }
               
            }
            await _inventoryDistributionRepositoryAsync.SaveChangesAsync();
        }
    }
}
