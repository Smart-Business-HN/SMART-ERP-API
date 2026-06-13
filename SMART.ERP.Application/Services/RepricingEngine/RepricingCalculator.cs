using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services.RepricingEngine
{
    /// <summary>
    /// Regla efectiva ya resuelta (override de producto o defaults globales) que consume el calculador.
    /// </summary>
    public sealed class EffectiveRepricingRule
    {
        public UndercutMode UndercutMode { get; init; } = UndercutMode.Percent;
        public decimal UndercutValue { get; init; } = 0.01m;
        public decimal MinMarginPercent { get; init; } = 0.15m;
        public PriceRoundingMode RoundingMode { get; init; } = PriceRoundingMode.None;
        public decimal MaxDecreasePercent { get; init; }
        public decimal MinChangeThreshold { get; init; }
        public bool AutoApply { get; init; }
    }

    public sealed class RepricingInput
    {
        /// <summary>Precio mínimo de competencia, ya normalizado a la misma base impositiva que el precio de la lista.</summary>
        public decimal MinCompetitorPrice { get; init; }
        /// <summary>Precio actual en la lista por defecto. 0 si el producto aún no tiene item (primer precio).</summary>
        public decimal CurrentPrice { get; init; }
        /// <summary>Costo del producto. 0 = sin info de costo (sin piso de margen).</summary>
        public decimal CostPrice { get; init; }
        public EffectiveRepricingRule Rule { get; init; } = new();
    }

    public sealed class RepricingOutcome
    {
        public decimal TargetPrice { get; init; }
        public decimal Floor { get; init; }
        public bool FloorHit { get; init; }
        public bool GuardTriggered { get; init; }
        /// <summary>True si <see cref="TargetPrice"/> es un precio válido y más bajo que el actual, listo para escribirse.</summary>
        public bool ShouldWrite { get; init; }
        public PriceChangeStatus Status { get; init; }
        public string? Reason { get; init; }
    }

    /// <summary>
    /// Lógica pura de re-fijación: calcula el precio objetivo a partir del mínimo de competencia, lo recorta
    /// al piso de margen, aplica redondeo y las guardas (umbral mínimo, caída máxima, no subir precio).
    /// Sin dependencias de BD/HTTP para poder verificarla con tests.
    /// </summary>
    public static class RepricingCalculator
    {
        public static RepricingOutcome Calculate(RepricingInput input)
        {
            var rule = input.Rule;

            // 1) Descuento crudo bajo el competidor más bajo.
            var raw = rule.UndercutMode == UndercutMode.Percent
                ? input.MinCompetitorPrice * (1m - rule.UndercutValue)
                : input.MinCompetitorPrice - rule.UndercutValue;

            // 2) Redondeo.
            var target = ApplyRounding(raw, rule.RoundingMode);

            // 3) Piso de margen.
            var floor = input.CostPrice > 0m
                ? Math.Round(input.CostPrice * (1m + rule.MinMarginPercent), 2)
                : 0m;

            var floorHit = false;
            if (floor > 0m && target < floor)
            {
                target = floor;
                floorHit = true;
            }
            target = Math.Round(target, 2);

            // Precio inválido.
            if (target <= 0m)
            {
                return new RepricingOutcome
                {
                    TargetPrice = target,
                    Floor = floor,
                    FloorHit = floorHit,
                    ShouldWrite = false,
                    Status = PriceChangeStatus.Skipped,
                    Reason = "El precio objetivo calculado es menor o igual a 0; no se escribe."
                };
            }

            // Producto sin precio previo: se fija por primera vez (sin guardas que dependan del precio actual).
            if (input.CurrentPrice <= 0m)
            {
                return new RepricingOutcome
                {
                    TargetPrice = target,
                    Floor = floor,
                    FloorHit = floorHit,
                    ShouldWrite = true,
                    Status = floorHit ? PriceChangeStatus.FloorHeld : PriceChangeStatus.Applied,
                    Reason = floorHit ? "Fijado al piso de margen (primer precio)." : null
                };
            }

            // Nunca subir el precio automáticamente: solo bajamos.
            if (target >= input.CurrentPrice)
            {
                return new RepricingOutcome
                {
                    TargetPrice = target,
                    Floor = floor,
                    FloorHit = floorHit,
                    ShouldWrite = false,
                    Status = floorHit ? PriceChangeStatus.FloorHeld : PriceChangeStatus.Skipped,
                    Reason = floorHit
                        ? "El piso de margen quedó por encima del precio actual; no se sube el precio."
                        : "Ya estamos igual o por debajo del competidor; sin cambio."
                };
            }

            // Guarda anti-espiral / anti-scrape-erróneo: caída demasiado grande en una sola corrida.
            if (rule.MaxDecreasePercent > 0m)
            {
                var maxDrop = input.CurrentPrice * rule.MaxDecreasePercent;
                if ((input.CurrentPrice - target) > maxDrop)
                {
                    return new RepricingOutcome
                    {
                        TargetPrice = target,
                        Floor = floor,
                        FloorHit = floorHit,
                        GuardTriggered = true,
                        ShouldWrite = false,
                        Status = PriceChangeStatus.RejectedByGuard,
                        Reason = $"Caída ({input.CurrentPrice - target:0.00}) supera el máximo permitido " +
                                 $"({maxDrop:0.00}); requiere revisión (posible lectura errónea)."
                    };
                }
            }

            // Umbral de ruido: cambio demasiado pequeño para molestarse en escribir.
            if (rule.MinChangeThreshold > 0m && (input.CurrentPrice - target) < rule.MinChangeThreshold)
            {
                return new RepricingOutcome
                {
                    TargetPrice = target,
                    Floor = floor,
                    FloorHit = floorHit,
                    ShouldWrite = false,
                    Status = PriceChangeStatus.Skipped,
                    Reason = "El cambio es menor al umbral mínimo configurado."
                };
            }

            return new RepricingOutcome
            {
                TargetPrice = target,
                Floor = floor,
                FloorHit = floorHit,
                ShouldWrite = true,
                Status = floorHit ? PriceChangeStatus.FloorHeld : PriceChangeStatus.Applied,
                Reason = floorHit ? "Bajado hasta el piso de margen." : null
            };
        }

        private static decimal ApplyRounding(decimal value, PriceRoundingMode mode)
        {
            switch (mode)
            {
                case PriceRoundingMode.Charm99:
                    var whole = Math.Floor(value);
                    var candidate = whole + 0.99m;
                    if (candidate > value) candidate = whole - 1m + 0.99m; // (whole-1).99
                    return candidate;
                case PriceRoundingMode.WholeNumber:
                    return Math.Floor(value); // hacia abajo, para no quedar por encima del competidor
                case PriceRoundingMode.None:
                default:
                    return Math.Round(value, 2);
            }
        }
    }
}
