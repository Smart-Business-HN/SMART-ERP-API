namespace SMART.ERP.Application.Services.RepricingEngine
{
    public interface IEcommerceRevalidationService
    {
        /// <summary>
        /// Pide al e-commerce (Next.js) revalidar las páginas de producto para que el precio nuevo aparezca
        /// antes del vencimiento del ISR. Best-effort: nunca lanza.
        /// </summary>
        Task RevalidateStoreAsync(CancellationToken ct = default);
    }
}
