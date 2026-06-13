using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Repricing
{
    public static class PriceChangeLogMapping
    {
        public static PriceChangeLogDto ToDto(this PriceChangeLog e) => new()
        {
            Id = e.Id,
            ProductId = e.ProductId,
            CompetitorSourceIdMin = e.CompetitorSourceIdMin,
            MinCompetitorPrice = e.MinCompetitorPrice,
            OldPrice = e.OldPrice,
            ProposedPrice = e.ProposedPrice,
            AppliedPrice = e.AppliedPrice,
            FloorHit = e.FloorHit,
            Applied = e.Applied,
            Status = e.Status,
            Reason = e.Reason,
            CreatedUtc = e.CreatedUtc,
            AppliedUtc = e.AppliedUtc,
            AppliedBy = e.AppliedBy
        };
    }
}
