using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Provider
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = null!;
        [Column(TypeName = "varchar(16)")]
        public string RTN { get; set; } = null!;
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string? ContactPerson { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string? ContactPhoneNumber { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? ContactEmail { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string Address { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string? WebsiteUrl { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
        public int TypeProviderId { get; set; }
        public virtual TypeProvider? TypeProvider { get; set; } = null!;
        public virtual List<PurchaseBill>? PurchaseBills { get; set; }
        public virtual List<NonBillableExpense>? NonBillableExpenses { get; set; }

        // Dropshipping support
        public bool SupportsDropshipping { get; set; }
        [Precision(18, 2)]
        public decimal? DefaultShippingCost { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? DefaultShippingType { get; set; }
        public int? DefaultShippingDays { get; set; }
        public virtual List<ProviderWarehouse>? ProviderWarehouses { get; set; }
        public virtual List<ShippingCostConfiguration>? ShippingCosts { get; set; }
        public bool CreditEnabled { get; set; }
        [Precision(18, 2)]
        public decimal CreditLimit { get; set; }
    }
}
