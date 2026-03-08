namespace SMART.ERP.Application.DTOs.Quotation
{
    public class QuotationPreviewDto
    {
        public int Id { get; set; }
        public string? QuotationCode { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? StatusName { get; set; }

        // Customer info (flattened, no internal IDs)
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerRtn { get; set; }

        // Sales advisor
        public string? UserFullName { get; set; }

        // Content
        public string? Observations { get; set; }
        public string? TermsAndConditions { get; set; }
        public List<ProductOfferedPreviewDto>? ProductsOffered { get; set; }

        // Totals
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalShippingCost { get; set; }
        public decimal SubTotalWithoutShipping { get; set; }

        // Feedback
        public List<QuotationCommentDto>? Comments { get; set; }
    }
}
