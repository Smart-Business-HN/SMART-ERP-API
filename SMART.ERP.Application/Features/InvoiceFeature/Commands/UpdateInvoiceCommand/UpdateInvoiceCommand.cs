using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.QuotationFeature.Commands.UpdateQuotationCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.ProductOfferedSpecification;
using SMART.ERP.Application.Specifications.ProductSoldSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InvoiceFeature.Commands.UpdateInvoiceCommand
{
    public class UpdateInvoiceCommand : IRequest<Response<InvoiceDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int BranchOfficeId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductToSellDto>? ProductsToSell { get; set; }
        public List<ProductSoldDto>? ProductsSold { get; set; }
        public int StatusId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
    }
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Response<InvoiceDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IRepositoryAsync<Cai> _caiRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<ProductSold> _productSoldRepositoryAsync;
        public UpdateInvoiceCommandHandler(IMapper mapper, IRepositoryAsync<Invoice> repositoryAsync, IRepositoryAsync<Cai> caiRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<ProductSold> productSoldRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _caiRepositoryAsync = caiRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _productSoldRepositoryAsync = productSoldRepositoryAsync;
        }
        public async Task<Response<InvoiceDto>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoiceExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterInvoiceByIdSpecification(request.Id));
            if (invoiceExist == null)
            {
                throw new ApiException($"No existe una factura con el Id {request.CustomerId}");
            }
            if (invoiceExist.Status.Name == "Gravada")
            {
                throw new ApiException($"La factura: {invoiceExist.InvoiceNumber} se encuentra cobrada y presentada al SAR.");
            }
            var customerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (customerExist == null)
            {
                throw new ApiException($"No existe un cliente con el Id {request.CustomerId}");
            }
            var branchOfficeExist = await _branchOfficeRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (branchOfficeExist == null)
            {
                throw new ApiException($"No existe una sucursal con el id {request.BranchOfficeId}");
            }
            var statusExist = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (statusExist == null)
            {
                throw new ApiException($"No existe un Estado con el id {request.StatusId}");
            }
            if (request.SagCode != null && (request.ExemptedRegistrationCertificateNumber == null || request.ExemptOrderNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            if (request.ExemptedRegistrationCertificateNumber != null && (request.SagCode == null || request.ExemptOrderNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            if (request.ExemptOrderNumber != null && (request.SagCode == null || request.ExemptedRegistrationCertificateNumber == null))
            {
                throw new ApiException($"Se requiere un registro de exoneracion completo: Codigo SAG, Numero de Orden Exenta, y Nº de Contancia de Registro Exonerado.");
            }
            //UPDATING VALUES
            if (invoiceExist.CustomerId != request.CustomerId)
            {
                invoiceExist.CustomerId = request.CustomerId;
                invoiceExist.Customer = null;
            }
            if (invoiceExist.BranchOfficeId != request.BranchOfficeId)
            {
                invoiceExist.BranchOfficeId = request.BranchOfficeId;
            }
            if (invoiceExist.StatusId != request.StatusId)
            {
                invoiceExist.StatusId = request.StatusId;
            }
            if (invoiceExist.Observations != request.Observations)
            {
                invoiceExist.Observations = request.Observations;
            }
            if (invoiceExist.TermsAndConditions != request.TermsAndConditions)
            {
                invoiceExist.TermsAndConditions = request.TermsAndConditions;
            }
            if( invoiceExist.PurchaseOrderCode != request.PurchaseOrderCode)
            {
                invoiceExist.PurchaseOrderCode = request.PurchaseOrderCode;
            }
            var taxesRates = await _taxRepositoryAsync.ListAsync();
            var productsSold = await CheckProducts(request.ProductsSold, request.ProductsToSell, request.Id, taxesRates);
            invoiceExist.Exempt = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 0));
            if (request.SagCode == null)
            {
                invoiceExist.TaxedAt15Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                invoiceExist.TaxedAt18Percent = CalculateGravableValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                invoiceExist.Taxes15Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15));
                invoiceExist.Taxes18Percent = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18));
                invoiceExist.Exonerated = 0;
                invoiceExist.Total = invoiceExist.TaxedAt15Percent + invoiceExist.TaxedAt18Percent + invoiceExist.Taxes15Percent + invoiceExist.Taxes18Percent;
                invoiceExist.Outstanding = invoiceExist.Total;
            }
            else
            {
                invoiceExist.Exonerated = CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 15)) + CalculateTaxesValue(productsSold, taxesRates.Find(x => x.Rate == 18));

            }
            invoiceExist.ProductsSold = null;
            await _repositoryAsync.UpdateAsync(invoiceExist);
            await _repositoryAsync.SaveChangesAsync();
            invoiceExist.Customer = customerExist;
            var dto = _mapper.Map<InvoiceDto>(invoiceExist);
            dto.ProductsSold = productsSold;
            return new Response<InvoiceDto>(dto, $"Cotizacion {invoiceExist.InvoiceNumber} actualizada exitosamente.");

        }
        static public decimal CalculateGravableValue(List<ProductSoldDto> products, Tax tax)
        {
            decimal gravable = 0;
            foreach (var product in products)
            {
                if (product.TaxId == tax.Id)
                {
                    gravable += (product.Quantity * product.UnitPrice);
                }
            }
            return gravable;
        }
        public static decimal CalculateTaxesValue(List<ProductSoldDto> products, Tax tax)
        {
            decimal taxes = 0;
            foreach (var product in products)
            {
                if (product.TaxId == tax.Id)
                {
                    decimal taxAmount = (product.Quantity * product.UnitPrice * (tax.Rate / 100));
                    taxes += taxAmount;
                }
            }
            return taxes;
        }
        public async Task<List<ProductSoldDto>> CheckProducts(List<ProductSoldDto> productSold, List<ProductToSellDto> productsToSell, int invoiceId, List<Tax> taxesRates)
        {
            var productsToProcess = new List<ProductToSellDto>(productsToSell);
            var prouductsPreExistence = new List<ProductSoldDto>(productSold);

            foreach (var productExisting in productSold)
            {
                //UPDATING EVERY PRODUCT AND REMOVING FROM LOCAL LISTS
                foreach (var item in productsToSell)
                {
                    if (productExisting.ProductId == item.ProductId)
                    {
                        var productToUpdate = productExisting;
                        if (productToUpdate.Quantity != item.Quantity)
                        {
                            productToUpdate.Quantity = item.Quantity;
                        }
                        if (productToUpdate.UnitPrice != item.RecomendedSalePrice)
                        {
                            productToUpdate.UnitPrice = item.RecomendedSalePrice;
                        }
                        productToUpdate.Taxes = TaxCalculator(item, taxesRates);
                        productToUpdate.TotalLine = productToUpdate.Taxes + (productToUpdate.Quantity * productToUpdate.UnitPrice);
                        productToUpdate.Product = null;
                        productToUpdate.Invoice = null;

                        productToUpdate.Tax = null;
                        var productSeed = _mapper.Map<ProductSold>(productToUpdate);
                        await _productSoldRepositoryAsync.UpdateAsync(productSeed);
                        await _productSoldRepositoryAsync.SaveChangesAsync();
                        prouductsPreExistence.RemoveAll(x => x.ProductId == productExisting.ProductId);
                        productsToProcess.RemoveAll(x => x.ProductId == item.ProductId);
                    }
                }
            }
            //ADD NEW PRODUCTS TO THE INVOICE
            foreach (var newProductToSell in productsToProcess)
            {
                var newProductOffered = _mapper.Map<ProductSold>(newProductToSell);
                newProductOffered.InvoiceId = invoiceId;
                newProductOffered.UnitPrice = newProductToSell.RecomendedSalePrice;
                newProductOffered.Taxes = TaxCalculator(newProductToSell, taxesRates);
                newProductOffered.TotalLine = newProductToSell.Quantity * newProductToSell.RecomendedSalePrice;
                await _productSoldRepositoryAsync.AddAsync(newProductOffered);
                await _productSoldRepositoryAsync.SaveChangesAsync();
            }
            //REMOVING PREVIOUS PRODUCTS FROM THE INVOICE
            foreach (var productPreExistence in prouductsPreExistence)
            {
                productPreExistence.Tax = null;
                productPreExistence.Invoice = null;
                productPreExistence.Product = null;

                var dtoToDelete = _mapper.Map<ProductSold>(productPreExistence);
                await _productSoldRepositoryAsync.DeleteAsync(dtoToDelete);
                await _productSoldRepositoryAsync.SaveChangesAsync();
            }
            //GETS NEW PRODUCTS FROM THE QUOTATION
            var newProducts = await _productSoldRepositoryAsync.ListAsync(new ProductSoldSpecification(invoiceId));
            var dtos = _mapper.Map<List<ProductSoldDto>>(newProducts);

            return dtos;
        }
        static public decimal TaxCalculator(ProductToSellDto product, List<Tax> taxes)
        {
            Tax productTax = taxes.Find(x => x.Id == product.TaxId);
            decimal gravable = product.Quantity * product.RecomendedSalePrice;
            decimal total = gravable * ((productTax!.Rate / 100) + 1);
            decimal tax = total - gravable;
            return tax;
        }
    }
}
