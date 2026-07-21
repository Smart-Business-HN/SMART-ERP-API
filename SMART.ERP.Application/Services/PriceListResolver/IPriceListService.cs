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

        /// <summary>
        /// Precios de UNA lista específica, <b>sin respaldo a la lista por defecto</b>.
        /// Un producto que no tenga fila en esa lista simplemente no aparece en el
        /// diccionario — el llamador decide qué hacer con él.
        ///
        /// Existe aparte de <see cref="GetPricesAsync"/> porque aquella, por diseño,
        /// sustituye en silencio por el precio de la lista por defecto y devuelve un
        /// diccionario plano sin procedencia. Esa semántica es correcta para vender
        /// ("sin precio especial, paga precio de lista") y es inaceptable para un
        /// documento publicable, donde imprimiría precios de público en un brochure
        /// rotulado "Distribuidor".
        ///
        /// Los valores YA INCLUYEN ISV (ver <see cref="Domain.Entities.PriceListItem.Price"/>).
        /// </summary>
        Task<IReadOnlyDictionary<int, decimal>> GetPricesFromListAsync(
            IEnumerable<int> productIds,
            int priceListId,
            CancellationToken ct = default);

        Task<int?> GetDefaultPriceListIdAsync(CancellationToken ct = default);
    }
}
