using SMART.ERP.Application.DTOs.Quotation;

namespace SMART.ERP.Application.Services.QuotationDiffService
{
    public class QuotationDiffService : IQuotationDiffService
    {
        public List<FieldChangeDto> ComputeDiff(QuotationSnapshotDataDto before, QuotationSnapshotDataDto after)
        {
            var changes = new List<FieldChangeDto>();

            CompareField(changes, "CustomerId", before.CustomerId.ToString(), after.CustomerId.ToString());
            CompareField(changes, "BranchOfficeId", before.BranchOfficeId.ToString(), after.BranchOfficeId.ToString());
            CompareField(changes, "UserId", before.UserId.ToString(), after.UserId.ToString());
            CompareField(changes, "StatusId", before.StatusId.ToString(), after.StatusId.ToString());
            CompareField(changes, "PrefixId", before.PrefixId.ToString(), after.PrefixId.ToString());
            CompareField(changes, "DueDate", before.DueDate.ToString("yyyy-MM-dd"), after.DueDate.ToString("yyyy-MM-dd"));
            CompareField(changes, "Observations", before.Observations, after.Observations);
            CompareField(changes, "TermsAndConditions", before.TermsAndConditions, after.TermsAndConditions);
            CompareField(changes, "ProjectId", before.ProjectId?.ToString(), after.ProjectId?.ToString());
            CompareField(changes, "SubTotal", before.SubTotal.ToString("F2"), after.SubTotal.ToString("F2"));
            CompareField(changes, "Total", before.Total.ToString("F2"), after.Total.ToString("F2"));
            CompareField(changes, "TotalShippingCost", before.TotalShippingCost.ToString("F2"), after.TotalShippingCost.ToString("F2"));
            CompareField(changes, "SubTotalWithoutShipping", before.SubTotalWithoutShipping.ToString("F2"), after.SubTotalWithoutShipping.ToString("F2"));

            CompareProducts(changes, before.ProductsOffered, after.ProductsOffered);

            return changes;
        }

        private static void CompareField(List<FieldChangeDto> changes, string fieldName, string? oldValue, string? newValue)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
            {
                changes.Add(new FieldChangeDto
                {
                    Field = fieldName,
                    OldValue = oldValue,
                    NewValue = newValue
                });
            }
        }

        private static void CompareProducts(List<FieldChangeDto> changes, List<ProductOfferedSnapshotDto> before, List<ProductOfferedSnapshotDto> after)
        {
            var beforeDict = before.ToDictionary(p => p.Id);
            var afterDict = after.ToDictionary(p => p.Id);

            // Products removed
            foreach (var old in before)
            {
                if (!afterDict.ContainsKey(old.Id))
                {
                    changes.Add(new FieldChangeDto
                    {
                        Field = $"Producto '{old.ProductDescription}' eliminado",
                        OldValue = $"Cant: {old.Quantity}, Precio: {old.UnitPrice:F2}",
                        NewValue = null
                    });
                }
            }

            // Products added
            foreach (var @new in after)
            {
                if (!beforeDict.ContainsKey(@new.Id))
                {
                    changes.Add(new FieldChangeDto
                    {
                        Field = $"Producto '{@new.ProductDescription}' agregado",
                        OldValue = null,
                        NewValue = $"Cant: {@new.Quantity}, Precio: {@new.UnitPrice:F2}"
                    });
                }
            }

            // Products modified
            foreach (var old in before)
            {
                if (afterDict.TryGetValue(old.Id, out var updated))
                {
                    var prefix = $"Producto '{old.ProductDescription}'";
                    CompareField(changes, $"{prefix}.Cantidad", old.Quantity.ToString("F2"), updated.Quantity.ToString("F2"));
                    CompareField(changes, $"{prefix}.PrecioUnitario", old.UnitPrice.ToString("F2"), updated.UnitPrice.ToString("F2"));
                    CompareField(changes, $"{prefix}.Impuestos", old.Taxes.ToString("F2"), updated.Taxes.ToString("F2"));
                    CompareField(changes, $"{prefix}.TotalLinea", old.TotalLine.ToString("F2"), updated.TotalLine.ToString("F2"));
                    CompareField(changes, $"{prefix}.Descripcion", old.ProductDescription, updated.ProductDescription);
                    CompareField(changes, $"{prefix}.CostoEnvio", old.ShippingCost.ToString("F2"), updated.ShippingCost.ToString("F2"));
                }
            }
        }
    }
}
