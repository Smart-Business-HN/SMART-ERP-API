namespace SMART.ERP.Application.Helpers
{
    /// <summary>
    /// Genera el folio de un documento de inventario a partir del formato del Prefix y el
    /// Id (único) del documento, rellenando con ceros hasta 8 caracteres. Usar el Id evita
    /// el frágil Max(Id) en memoria del módulo legacy y garantiza unicidad.
    /// </summary>
    public static class InventoryCodeGenerator
    {
        public static string Generate(string prefixFormat, int id)
        {
            var idText = id.ToString();
            var totalLength = prefixFormat.Length + idText.Length;
            if (totalLength < 8)
            {
                var padding = 8 - totalLength;
                return prefixFormat + new string('0', padding) + idText;
            }
            return prefixFormat + idText;
        }
    }
}
