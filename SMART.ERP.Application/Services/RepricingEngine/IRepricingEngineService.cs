using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.RepricingEngine
{
    public interface IRepricingEngineService
    {
        /// <summary>
        /// Lee las fuentes de competencia habilitadas del producto, calcula el precio objetivo, escribe la
        /// bitácora y —si corresponde— aplica el precio a la lista por defecto. No dispara la revalidación
        /// del e-commerce (eso lo hace el llamador una vez por lote). Devuelve la bitácora generada, o
        /// <c>null</c> si no había nada que evaluar (sin fuentes / producto excluido).
        /// </summary>
        Task<PriceChangeLog?> EvaluateAndApplyAsync(int productId, string actor, CancellationToken ct = default);
    }
}
