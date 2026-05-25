namespace SMART.ERP.Application.Services.ProductCacheInvalidator
{
    /// <summary>
    /// Invalida todas las políticas de caché relacionadas con productos (admin y ecommerce).
    /// Se llama tras cambios de stock o costo provocados por movimientos de inventario.
    /// </summary>
    public interface IProductCacheInvalidator
    {
        Task InvalidateAsync(CancellationToken cancellationToken = default);
    }
}
