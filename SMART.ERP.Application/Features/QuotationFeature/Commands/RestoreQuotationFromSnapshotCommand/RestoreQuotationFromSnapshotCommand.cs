using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductOfferedSpecification;
using SMART.ERP.Application.Specifications.QuotationSnapshotSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.RestoreQuotationFromSnapshotCommand
{
    public class RestoreQuotationFromSnapshotCommand : IRequest<Response<QuotationDto>>
    {
        public int QuotationId { get; set; }
        public int SnapshotId { get; set; }
    }
    public class RestoreQuotationFromSnapshotCommandHandler : IRequestHandler<RestoreQuotationFromSnapshotCommand, Response<QuotationDto>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IRepositoryAsync<QuotationSnapshot> _snapshotRepositoryAsync;
        private readonly IRepositoryAsync<ProductOffered> _productOfferedRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        public RestoreQuotationFromSnapshotCommandHandler(
            IRepositoryAsync<Quotation> repositoryAsync,
            IRepositoryAsync<QuotationSnapshot> snapshotRepositoryAsync,
            IRepositoryAsync<ProductOffered> productOfferedRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IMapper mapper,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _snapshotRepositoryAsync = snapshotRepositoryAsync;
            _productOfferedRepositoryAsync = productOfferedRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<Response<QuotationDto>> Handle(RestoreQuotationFromSnapshotCommand request, CancellationToken cancellationToken)
        {
            // Validate snapshot exists and belongs to the quotation
            var snapshot = await _snapshotRepositoryAsync.FirstOrDefaultAsync(new GetQuotationSnapshotByIdSpecification(request.SnapshotId));
            if (snapshot == null)
            {
                throw new ApiException($"No existe un snapshot con el Id {request.SnapshotId}");
            }
            if (snapshot.QuotationId != request.QuotationId)
            {
                throw new ApiException($"El snapshot {request.SnapshotId} no pertenece a la cotización {request.QuotationId}");
            }

            // Get current quotation with products
            var quotation = await _repositoryAsync.FirstOrDefaultAsync(new FilterQuotationByIdSpecification(request.QuotationId));
            if (quotation == null)
            {
                throw new ApiException($"No existe una cotización con el Id {request.QuotationId}");
            }

            // Create pre-restore snapshot (so restore is reversible)
            var currentSnapshot = BuildSnapshotData(quotation);
            var lastVersionList = await _snapshotRepositoryAsync.ListAsync(new GetMaxVersionSpecification(request.QuotationId));
            int nextVersion = (lastVersionList.FirstOrDefault()?.VersionNumber ?? 0) + 1;

            var preRestoreSnapshot = new QuotationSnapshot
            {
                QuotationId = request.QuotationId,
                VersionNumber = nextVersion,
                SnapshotData = JsonConvert.SerializeObject(currentSnapshot),
                ChangeSummary = JsonConvert.SerializeObject(new List<FieldChangeDto>
                {
                    new() { Field = "Restauración", OldValue = $"Versión actual", NewValue = $"Restaurado a versión {snapshot.VersionNumber}" }
                }),
                CreatedBy = _jwtService.GetSubjectToken(),
                CreatedDate = DateTime.UtcNow
            };
            await _snapshotRepositoryAsync.AddAsync(preRestoreSnapshot);
            await _snapshotRepositoryAsync.SaveChangesAsync();

            // Deserialize the snapshot data to restore
            var snapshotData = JsonConvert.DeserializeObject<QuotationSnapshotDataDto>(snapshot.SnapshotData)!;

            // Restore parent scalar fields
            quotation.CustomerId = snapshotData.CustomerId;
            quotation.BranchOfficeId = snapshotData.BranchOfficeId;
            quotation.UserId = snapshotData.UserId;
            quotation.DueDate = snapshotData.DueDate;
            quotation.Observations = snapshotData.Observations;
            quotation.TermsAndConditions = snapshotData.TermsAndConditions;
            quotation.SubTotal = snapshotData.SubTotal;
            quotation.Total = snapshotData.Total;
            quotation.StatusId = snapshotData.StatusId;
            quotation.PrefixId = snapshotData.PrefixId;
            quotation.Profitability = snapshotData.Profitability;
            quotation.ProjectId = snapshotData.ProjectId;
            quotation.TotalShippingCost = snapshotData.TotalShippingCost;
            quotation.SubTotalWithoutShipping = snapshotData.SubTotalWithoutShipping;
            quotation.ModificatedBy = _jwtService.GetSubjectToken();
            quotation.ModificationDate = DateTime.UtcNow;

            // Delete all current products
            if (quotation.ProductsOffered != null)
            {
                foreach (var product in quotation.ProductsOffered)
                {
                    product.Tax = null;
                    product.Quotation = null;
                    product.Product = null;
                    product.SourceWarehouse = null;
                    await _productOfferedRepositoryAsync.DeleteAsync(product);
                    await _productOfferedRepositoryAsync.SaveChangesAsync();
                }
            }

            // Null out navigation properties before save
            quotation.ProductsOffered = null;
            quotation.Customer = null;
            quotation.User = null;
            quotation.Status = null;
            quotation.BranchOffice = null;
            quotation.Prefix = null;
            quotation.InvoiceDestination = null;
            quotation.Project = null;

            await _repositoryAsync.UpdateAsync(quotation);
            await _repositoryAsync.SaveChangesAsync();

            // Re-create products from snapshot
            foreach (var productSnapshot in snapshotData.ProductsOffered)
            {
                var newProduct = new ProductOffered
                {
                    QuotationId = request.QuotationId,
                    ProductId = productSnapshot.ProductId,
                    ProductCode = productSnapshot.ProductCode,
                    ProductDescription = productSnapshot.ProductDescription,
                    UnitPrice = productSnapshot.UnitPrice,
                    Quantity = productSnapshot.Quantity,
                    TaxId = productSnapshot.TaxId,
                    Taxes = productSnapshot.Taxes,
                    TotalLine = productSnapshot.TotalLine,
                    SourceWarehouseId = productSnapshot.SourceWarehouseId,
                    ShippingCost = productSnapshot.ShippingCost,
                    SubTotalWithoutShipping = productSnapshot.SubTotalWithoutShipping,
                    IsFromVirtualStock = productSnapshot.IsFromVirtualStock
                };
                await _productOfferedRepositoryAsync.AddAsync(newProduct);
                await _productOfferedRepositoryAsync.SaveChangesAsync();
            }

            // Fetch restored quotation for response
            var restoredQuotation = await _repositoryAsync.FirstOrDefaultAsync(new FilterQuotationByIdSpecification(request.QuotationId));
            var dto = _mapper.Map<QuotationDto>(restoredQuotation);
            var products = await _productOfferedRepositoryAsync.ListAsync(new ProductOfferedSpecification(request.QuotationId));
            dto.ProductsOffered = _mapper.Map<List<ProductOfferedDto>>(products);

            return new Response<QuotationDto>(dto, $"Cotización restaurada exitosamente a la versión {snapshot.VersionNumber}.");
        }

        private QuotationSnapshotDataDto BuildSnapshotData(Quotation quotation)
        {
            var snapshotData = _mapper.Map<QuotationSnapshotDataDto>(quotation);
            if (quotation.ProductsOffered != null)
            {
                snapshotData.ProductsOffered = _mapper.Map<List<ProductOfferedSnapshotDto>>(quotation.ProductsOffered);
            }
            return snapshotData;
        }
    }
}
