using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class Quotation
    {
        public int Id { get; init; }
        public Guid CustomerId { get; set; }
        public string? QuotationCode { get; set; }
        public virtual Customer? Customer { get; set; }
        public int BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductOffered>? ProductsOffered { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int PrefixId { get; set; }
        public virtual Prefix? Prefix { get; set; }
        public decimal? Profitability { get; set; }

    }
}
