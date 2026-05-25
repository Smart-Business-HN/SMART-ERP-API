using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Helpers
{
    /// <summary>
    /// Reglas de estructura del catálogo de cuentas (NIIF para PYMES — Honduras):
    /// Tipo=1, Grupo=2, Mayor=4, SubCuenta=7, Auxiliar=9. El máximo es 9 dígitos; auxiliares
    /// solo para casos estructurales (cuentas bancarias específicas, categorías de inventario).
    /// </summary>
    public static class AccountCodeHelper
    {
        public static AccountLevel ResolveLevel(string code)
        {
            return code.Length switch
            {
                1 => AccountLevel.Tipo,
                2 => AccountLevel.Grupo,
                4 => AccountLevel.Mayor,
                7 => AccountLevel.SubCuenta,
                9 => AccountLevel.Auxiliar,
                _ => throw new ArgumentException(
                    $"La longitud del código '{code}' no es válida. Use 1 (Tipo), 2 (Grupo), 4 (Mayor), 7 (SubCuenta) o 9 (Auxiliar).")
            };
        }

        public static NormalBalanceSide ResolveNormalBalanceSide(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Activo => NormalBalanceSide.Debit,
                AccountType.Costos => NormalBalanceSide.Debit,
                AccountType.Gastos => NormalBalanceSide.Debit,
                AccountType.Pasivo => NormalBalanceSide.Credit,
                AccountType.Patrimonio => NormalBalanceSide.Credit,
                AccountType.Ingresos => NormalBalanceSide.Credit,
                _ => NormalBalanceSide.Debit // Cuentas de Orden: deudoras por defecto
            };
        }
    }
}
