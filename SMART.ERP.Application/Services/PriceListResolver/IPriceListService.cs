using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.PriceListResolver
{
    public interface IPriceListService
    {
        Task<int?> ResolvePriceListIdAsync(
            Guid? customerId = null,
            int? customerTypeId = null,
            bool isAnonymous = false,
            CancellationToken ct = default);

        Task<decimal?> GetPriceAsync(
            int productId,
            int? resolvedPriceListId,
            CancellationToken ct = default);

        Task<IReadOnlyDictionary<int, decimal>> GetPricesAsync(
            IEnumerable<int> productIds,
            int? resolvedPriceListId,
            CancellationToken ct = default);

        Task<int?> GetDefaultPriceListIdAsync(CancellationToken ct = default);
    }
}
