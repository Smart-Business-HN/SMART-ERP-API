using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.CompetitorScraper
{
    public interface ICompetitorScraperService
    {
        /// <summary>
        /// Lee el precio actual de una fuente de competencia según su <see cref="CompetitorSource.ParseStrategy"/>.
        /// Nunca lanza: ante cualquier fallo retorna <see cref="ScrapeResult.Fail"/>.
        /// </summary>
        Task<ScrapeResult> ScrapeAsync(CompetitorSource source, CancellationToken ct = default);
    }
}
