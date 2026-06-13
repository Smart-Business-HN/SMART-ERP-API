using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services.RepricingEngine
{
    /// <summary>
    /// Normaliza precios de competencia a una base comparable. Convención del sistema:
    /// el precio de la lista (<c>PriceListItem.Price</c>) se muestra al cliente con ISV incluido,
    /// igual que los precios de retail de la competencia, así que la base comparable es "ISV incluido".
    /// </summary>
    public static class CompetitorPriceNormalizer
    {
        private const decimal Isv15 = 0.15m;

        public static decimal ToComparable(decimal rawPrice, CompetitorTaxBasis basis)
        {
            return basis == CompetitorTaxBasis.ExcludesTax
                ? Math.Round(rawPrice * (1m + Isv15), 2)
                : rawPrice;
        }
    }
}
