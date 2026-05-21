using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using SMART.ERP.Application.Repository;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.PriceListResolver
{
    public class PriceListService : IPriceListService
    {
        private readonly IReadRepositoryAsync<PriceList> _priceListRepo;
        private readonly IReadRepositoryAsync<PriceListItem> _priceListItemRepo;
        private readonly IReadRepositoryAsync<Customer> _customerRepo;
        private readonly IReadRepositoryAsync<CustomerType> _customerTypeRepo;
        private readonly ILogger<PriceListService> _logger;

        public PriceListService(
            IReadRepositoryAsync<PriceList> priceListRepo,
            IReadRepositoryAsync<PriceListItem> priceListItemRepo,
            IReadRepositoryAsync<Customer> customerRepo,
            IReadRepositoryAsync<CustomerType> customerTypeRepo,
            ILogger<PriceListService> logger)
        {
            _priceListRepo = priceListRepo;
            _priceListItemRepo = priceListItemRepo;
            _customerRepo = customerRepo;
            _customerTypeRepo = customerTypeRepo;
            _logger = logger;
        }

        public async Task<int?> ResolvePriceListIdAsync(
            Guid? customerId = null,
            int? customerTypeId = null,
            bool isAnonymous = false,
            CancellationToken ct = default)
        {
            if (customerId.HasValue)
            {
                var customer = await _customerRepo.FirstOrDefaultAsync(
                    new CustomerByIdSpec(customerId.Value), ct);
                if (customer is not null)
                {
                    if (customer.PriceListId.HasValue)
                        return customer.PriceListId;

                    customerTypeId ??= customer.CustomerTypeId;
                }
            }

            if (customerTypeId.HasValue)
            {
                var ct2 = await _customerTypeRepo.FirstOrDefaultAsync(
                    new CustomerTypeByIdSpec(customerTypeId.Value), ct);
                if (ct2?.PriceListId.HasValue == true)
                    return ct2.PriceListId;
            }

            return await GetDefaultPriceListIdAsync(ct);
        }

        public async Task<decimal?> GetPriceAsync(
            int productId,
            int? resolvedPriceListId,
            CancellationToken ct = default)
        {
            var defaultId = await GetDefaultPriceListIdAsync(ct);
            var ids = new List<int>();
            if (resolvedPriceListId.HasValue) ids.Add(resolvedPriceListId.Value);
            if (defaultId.HasValue && defaultId != resolvedPriceListId) ids.Add(defaultId.Value);
            if (ids.Count == 0) return null;

            var items = await _priceListItemRepo.ListAsync(
                new PriceListItemsByListsAndProductsSpec(ids, new[] { productId }), ct);

            if (items.Count == 0) return null;

            if (resolvedPriceListId.HasValue)
            {
                var match = items.FirstOrDefault(x => x.PriceListId == resolvedPriceListId.Value);
                if (match is not null) return match.Price;
            }

            if (defaultId.HasValue)
            {
                var match = items.FirstOrDefault(x => x.PriceListId == defaultId.Value);
                if (match is not null) return match.Price;
            }

            return null;
        }

        public async Task<IReadOnlyDictionary<int, decimal>> GetPricesAsync(
            IEnumerable<int> productIds,
            int? resolvedPriceListId,
            CancellationToken ct = default)
        {
            var productIdsList = productIds.Distinct().ToList();
            var result = new Dictionary<int, decimal>();
            if (productIdsList.Count == 0) return result;

            var defaultId = await GetDefaultPriceListIdAsync(ct);
            var ids = new List<int>();
            if (resolvedPriceListId.HasValue) ids.Add(resolvedPriceListId.Value);
            if (defaultId.HasValue && defaultId != resolvedPriceListId) ids.Add(defaultId.Value);
            if (ids.Count == 0) return result;

            var items = await _priceListItemRepo.ListAsync(
                new PriceListItemsByListsAndProductsSpec(ids, productIdsList), ct);

            foreach (var productId in productIdsList)
            {
                if (resolvedPriceListId.HasValue)
                {
                    var match = items.FirstOrDefault(x =>
                        x.PriceListId == resolvedPriceListId.Value && x.ProductId == productId);
                    if (match is not null)
                    {
                        result[productId] = match.Price;
                        continue;
                    }
                }
                if (defaultId.HasValue)
                {
                    var match = items.FirstOrDefault(x =>
                        x.PriceListId == defaultId.Value && x.ProductId == productId);
                    if (match is not null)
                        result[productId] = match.Price;
                }
            }

            return result;
        }

        public async Task<int?> GetDefaultPriceListIdAsync(CancellationToken ct = default)
        {
            var defaultList = await _priceListRepo.FirstOrDefaultAsync(new DefaultPriceListSpec(), ct);
            if (defaultList is null)
            {
                _logger.LogWarning("No PriceList marked as IsDefault found. Prices may be unavailable.");
                return null;
            }
            return defaultList.Id;
        }

        private sealed class DefaultPriceListSpec : Specification<PriceList>
        {
            public DefaultPriceListSpec()
            {
                Query.Where(x => x.IsDefault && x.IsActive);
            }
        }

        private sealed class CustomerByIdSpec : Specification<Customer>
        {
            public CustomerByIdSpec(Guid id)
            {
                Query.Where(x => x.Id == id);
            }
        }

        private sealed class CustomerTypeByIdSpec : Specification<CustomerType>
        {
            public CustomerTypeByIdSpec(int id)
            {
                Query.Where(x => x.Id == id);
            }
        }

        private sealed class PriceListItemsByListsAndProductsSpec : Specification<PriceListItem>
        {
            public PriceListItemsByListsAndProductsSpec(IReadOnlyCollection<int> listIds, IReadOnlyCollection<int> productIds)
            {
                Query.Where(x => listIds.Contains(x.PriceListId) && productIds.Contains(x.ProductId));
            }
        }
    }
}
