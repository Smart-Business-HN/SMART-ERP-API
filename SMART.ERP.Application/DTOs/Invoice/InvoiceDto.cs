using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int CaiId { get; set; }
        public CaiDto? Cai { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public int? QuotationOriginId { get; set; }
        public QuotationDto? Quotation { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public BranchOfficeDto? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public DateTime CreationDate { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? SagCode { get; set; }
        public string? ExemptOrderNumber { get; set; }
        public string? ExemptedRegistrationCertificateNumber { get; set; }
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public decimal Outstanding { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public List<ProductSoldDto>? ProductsSold { get; set; }
        public int DestinationInvoiceId { get; set; }
        public InvoiceDto? DestinationInvoice { get; set; }
    }
}
