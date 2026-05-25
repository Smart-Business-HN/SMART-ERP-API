namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Nivel jerárquico de la cuenta en el catálogo. Convención de longitud de código:
    /// Tipo=1 dígito, Grupo=2, Mayor=4, SubCuenta=7, Auxiliar=variable (mayor a 7).
    /// </summary>
    public enum AccountLevel
    {
        Tipo = 1,
        Grupo = 2,
        Mayor = 3,
        SubCuenta = 4,
        Auxiliar = 5
    }
}
