namespace SMART.ERP.Application.Services.ProductCompositionService
{
    public interface IProductCompositionService
    {
        /// <summary>
        /// Resuelve la lista de movimientos de Kardex que deben generarse al vender una
        /// cantidad de un producto. Para Tangible devuelve una sola línea con el propio
        /// producto. Para Combo expande la receta (parts activas) multiplicando por la
        /// cantidad vendida y devuelve una línea por componente con su costo. Para Service
        /// devuelve lista vacía (no afecta inventario).
        /// </summary>
        Task<List<ComponentMovement>> ResolveComponentsForSaleAsync(int productId, decimal saleQuantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calcula el stock disponible de un combo como min(stock_componente / cantidad_receta)
        /// truncado al entero inferior. Devuelve 0 si no tiene componentes activos.
        /// </summary>
        Task<decimal> GetCalculatedComboStockAsync(int comboId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Versión bulk: para una lista de combos, devuelve un diccionario {comboId => stock calculado}
        /// minimizando consultas a la base de datos.
        /// </summary>
        Task<Dictionary<int, decimal>> GetCalculatedStockMapAsync(IEnumerable<int> comboIds, CancellationToken cancellationToken = default);
    }
}
