using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.RepricingSpecification
{
    /// <summary>Fuentes de competencia de un producto (incluye Product). Para la pantalla de detalle.</summary>
    public class CompetitorSourcesByProductSpecification : Specification<CompetitorSource>
    {
        public CompetitorSourcesByProductSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId)
                 .OrderBy(x => x.CompetitorName)
                 .AsNoTracking();
        }
    }

    /// <summary>Fuentes habilitadas de un producto. Para el motor de re-fijación.</summary>
    public class EnabledCompetitorSourcesByProductSpecification : Specification<CompetitorSource>
    {
        public EnabledCompetitorSourcesByProductSpecification(int productId)
        {
            // Sin Include(Product): el motor carga el producto por separado y nunca usa la navegación.
            // Incluirla provoca un conflicto de change-tracking al hacer Update de varias fuentes del
            // mismo producto (con NoTracking cada fuente trae su propia instancia de Product → choque de claves).
            Query.Where(x => x.IsEnabled && x.ProductId == productId);
        }
    }

    /// <summary>Una fuente concreta por (producto, competidor) — para el upsert idempotente.</summary>
    public class CompetitorSourceByProductAndNameSpecification : Specification<CompetitorSource>
    {
        public CompetitorSourceByProductAndNameSpecification(int productId, string competitorName)
        {
            Query.Where(x => x.ProductId == productId && x.CompetitorName == competitorName);
        }
    }

    /// <summary>Ids de producto distintos que tienen al menos una fuente habilitada. Lo usa el job.</summary>
    public class ProductsWithEnabledSourcesSpecification : Specification<CompetitorSource>
    {
        public ProductsWithEnabledSourcesSpecification()
        {
            Query.Where(x => x.IsEnabled).AsNoTracking();
        }
    }

    /// <summary>Todas las fuentes habilitadas con su Product. Para el dashboard de re-fijación.</summary>
    public class AllEnabledCompetitorSourcesWithProductSpecification : Specification<CompetitorSource>
    {
        public AllEnabledCompetitorSourcesWithProductSpecification()
        {
            Query.Where(x => x.IsEnabled)
                 .Include(x => x.Product!)
                 .AsNoTracking();
        }
    }

    public class RepricingRulesByProductsSpecification : Specification<RepricingRule>
    {
        public RepricingRulesByProductsSpecification(IReadOnlyCollection<int> productIds)
        {
            Query.Where(x => productIds.Contains(x.ProductId)).AsNoTracking();
        }
    }

    /// <summary>Productos por Ids (código/nombre). Para el reporte por correo del job.</summary>
    public class ProductsByIdsSpecification : Specification<Product>
    {
        public ProductsByIdsSpecification(IReadOnlyCollection<int> productIds)
        {
            Query.Where(x => productIds.Contains(x.Id)).AsNoTracking();
        }
    }

    public class RepricingRuleByProductSpecification : Specification<RepricingRule>
    {
        public RepricingRuleByProductSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId);
        }
    }

    /// <summary>Config global (fila única).</summary>
    public class RepricingSettingsSingletonSpecification : Specification<RepricingSettings>
    {
        public RepricingSettingsSingletonSpecification()
        {
            Query.OrderBy(x => x.Id);
        }
    }

    /// <summary>Bitácora de cambios de precio, paginada y filtrable por producto.</summary>
    public class PriceChangeLogPagedSpecification : Specification<PriceChangeLog>
    {
        public PriceChangeLogPagedSpecification(int? productId, int pageNumber, int pageSize)
        {
            Query.AsNoTracking();
            if (productId.HasValue)
                Query.Where(x => x.ProductId == productId.Value);

            Query.OrderByDescending(x => x.CreatedUtc)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize);
        }
    }

    public class CountPriceChangeLogSpecification : Specification<PriceChangeLog>
    {
        public CountPriceChangeLogSpecification(int? productId)
        {
            Query.AsNoTracking();
            if (productId.HasValue)
                Query.Where(x => x.ProductId == productId.Value);
        }
    }

    /// <summary>Última evaluación registrada para un producto (para el dashboard).</summary>
    public class LatestPriceChangeLogByProductSpecification : Specification<PriceChangeLog>
    {
        public LatestPriceChangeLogByProductSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId)
                 .OrderByDescending(x => x.CreatedUtc)
                 .Take(1)
                 .AsNoTracking();
        }
    }
}
