using SMART.ERP.Application.DTOs.Brochure;

namespace SMART.ERP.Application.Services.BrochurePdfService
{
    public interface IBrochurePdfService
    {
        /// <summary>
        /// Compone el brochure. El DTO debe llegar completamente materializado
        /// (precios finales, textos saneados, imágenes en memoria): este servicio no
        /// hace I/O ni consulta nada.
        /// </summary>
        Task<byte[]> GeneratePdfAsync(BrochureDocumentDto document);
    }
}
