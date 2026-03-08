using SMART.ERP.Application.DTOs.Quotation;

namespace SMART.ERP.Application.Services.QuotationDiffService
{
    public interface IQuotationDiffService
    {
        List<FieldChangeDto> ComputeDiff(QuotationSnapshotDataDto before, QuotationSnapshotDataDto after);
    }
}
