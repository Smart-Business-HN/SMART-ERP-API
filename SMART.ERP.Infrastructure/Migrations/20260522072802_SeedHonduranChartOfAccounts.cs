using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <summary>
    /// Siembra el catálogo de cuentas hondureño por defecto (estructura Zafra Cloud / NIIF para PYMES).
    /// Tipo(1) → Grupo(2) → Mayor(4) → SubCuenta(7) → Auxiliar(>7). Solo las hojas a nivel Mayor o
    /// inferior quedan como imputables. El catálogo es editable por cada empresa.
    /// </summary>
    public partial class SeedHonduranChartOfAccounts : Migration
    {
        // Naturaleza por dígito Tipo: 1 Activo, 2 Pasivo, 3 Patrimonio, 4 Ingresos, 5 Costos, 6 Gastos, 7 Cuentas de Orden.
        private static readonly (string Code, string Name)[] Accounts =
        {
            ("1", "ACTIVO"),
            ("11", "ACTIVO CORRIENTE"),
            ("1101", "EFECTIVO Y EQUIVALENTE A EFECTIVO"),
            ("1101001", "CAJA GENERAL"),
            ("1101002", "CAJA CHICA"),
            ("1101003", "BANCOS MONEDA NACIONAL"),
            ("110100301", "Lempiras BAC"),
            ("1101004", "BANCOS MONEDA EXTRANJERA"),
            ("1102", "CUENTAS POR COBRAR"),
            ("1102001", "COMERCIALES"),
            ("1102002", "CLIENTES"),
            ("11020020001", "Consumidor Final"),
            ("1102003", "PROVEEDORES"),
            ("1102004", "ANTICIPO A PROVEEDORES"),
            ("1102005", "PRESTAMOS AL PERSONAL"),
            ("1102006", "PRESTAMOS A SOCIOS"),
            ("1102007", "OTRAS CUENTAS POR COBRAR"),
            ("1103", "CUENTAS POR COBRAR COMPAÑIAS Y PARTES RELACIONADAS"),
            ("1103001", "AFILIADAS"),
            ("1104", "CUENTAS POR COBRAR ACCIONISTAS"),
            ("1104001", "ACCIONES SUSCRITAS Y NO PAGADAS"),
            ("1105", "DOCUMENTOS POR COBRAR"),
            ("1105001", "PAGARES/ LETRAS DE CAMBIO"),
            ("1106", "DOCUMENTOS POR COBRAR COMPAÑIAS Y PARTES RELACIONADAS"),
            ("1106001", "PAGARES/ LETRAS DE CAMBIO"),
            ("1107", "INTERESES POR COBRAR"),
            ("1107001", "CLIENTES"),
            ("1107002", "AFILIADAS"),
            ("1108", "INVENTARIOS"),
            ("1108001", "INVENTARIO DE PRODUCTOS TERMINADOS"),
            ("1108002", "INVENTARIO DE MATERIA PRIMA"),
            ("1108003", "INVENTARIO DE PRODUCCION EN PROCESO"),
            ("1108004", "MERCADERIA EN TRANSITO"),
            ("1108005", "ANTICIPO A PROVEEDORES"),
            ("1108006", "AJUSTE DE INCREMENTO"),
            ("1108007", "PRODUCCION Y CREACION"),
            ("1108008", "VENTA"),
            ("1108009", "AJUSTE DE DISMINUCION"),
            ("1108010", "INVENTARIO FISICO"),
            ("1108011", "PRODUCTO EN PROCESO"),
            ("1108012", "MERCADERIA EN TRANSITO"),
            ("1108013", "MATERIA PRIMA"),
            ("1109", "INVERSIONES TEMPORALES"),
            ("1109001", "INVERSIONES EN AFILIADAS"),
            ("1110", "PAGOS ANTICIPADOS"),
            ("1110001", "PRIMAS DE SEGUROS"),
            ("1110002", "ALQUILERES PAGADOS POR ANTICIPADO"),
            ("1110003", "OTRAS CUENTAS PAGAS POR ANTICIPADO"),
            ("1111", "OTROS ACTIVOS CIRCULANTES"),
            ("12", "ACTIVO FIJO"),
            ("1201", "PROPIEDAD PLANTA Y EQUIPO"),
            ("1201001", "TERRENOS"),
            ("1201002", "EDIFICIOS"),
            ("1201003", "MOBILIARIO Y EQUIPO"),
            ("1201004", "VEHICULOS"),
            ("1202", "INVERSIONES TEMPORALES"),
            ("1202001", "ACCIONES EN AFILIADAS"),
            ("1203", "INVERSIONES PERMANENTES"),
            ("1203001", "EN AFILIADAS"),
            ("1203002", "EN ASOCIADAS"),
            ("1204", "IMPUESTOS PAGADOS POR ANTICIPADOS"),
            ("1204001", "RETENCION 1%"),
            ("1204002", "RETENCION 15%"),
            ("1205", "ACTIVOS INTANGIBLES"),
            ("1205001", "MARCAS Y PATENTES"),
            ("1206", "CUENTAS POR COBRAR A LARGO PLAZO"),
            ("1206001", "CUENTAS POR COBRAR A LARGO PLAZO COMERCIALES"),
            ("1207", "PRESTAMOS A ACCIONISTAS A LARGO PLAZO"),
            ("1207001", "PRESTAMOS ACCIONISTA A LARGO PLAZO"),
            ("1208", "VALORES EN GARANTIA"),
            ("1208001", "DEPOSITOS EN GARANTIA"),
            ("1209", "REVALUACION DE PROPIEDAD PLANTA Y EQUIPO"),
            ("1209001", "REVALUACION DE TERRENOS"),
            ("1209002", "REVALUACION DE EDIFICIOS"),
            ("1209003", "REVALUACION DE MOBILIARIO Y EQUIPO"),
            ("1209004", "REVALUACION DE VEHICULOS"),
            ("1210", "DEPRECIACION ACUMULADA"),
            ("1210001", "DEPRECIACION ACUMULADA DE EDIFICIOS"),
            ("1210002", "DEPRECIACION ACUMULADA DE MOBILIARIO"),
            ("1210003", "DEPRECIACION ACUMULADA DE VEHICULO"),
            ("1210004", "DEPRECIACION ACUMULADA DE MAQUINARIA"),
            ("1210005", "AMORTIZACION ACUMULADA"),
            ("13", "ACTIVO DIFERIDO"),
            ("1301", "GASTOS ANTICIPADOS"),
            ("1301001", "PRIMAS DE SEGUROS"),
            ("1301002", "ALQUILERES PAGADOS POR ANTICIPADO"),
            ("1301003", "RESERVA RABORAL RAP 4%"),
            ("1302", "CREDITO FISCALES E IMPUESTOS PAGADOS POR ANTICIPADOS"),
            ("1302001", "CREDITO FISCAL 15% ISV POR COMPRAS"),
            ("1302002", "CREDITO FISCAL 1% ISR"),
            ("1302003", "CREDITO FISCAL 15% ISV (ART. 8)"),
            ("1302004", "PAGOS A CUENTA ISR"),
            ("1302005", "CREDITO FISCAL P/VENTAS CON TARJETAS DE CREDITO"),
            ("1303", "IMPUESTOS PAGADOS POR ANTICIPADOS"),
            ("1303001", "PRIMAS DE SEGUROS"),
            ("1303002", "ALQUILERES PAGADOS POR ANTICIPADOS"),
            ("1304", "VALORES EN GARANTIA"),

            ("2", "PASIVO"),
            ("21", "PASIVO CIRCULANTE"),
            ("2101", "CUENTAS POR PAGAR"),
            ("2101001", "PROVEEDORES"),
            ("21010010001", "Microsoft"),
            ("2101002", "ACREEDORES VARIOS"),
            ("2101003", "ALQUILERES RECIBIDOS POR ANTICIPADO"),
            ("2101004", "OTRAS CUENTAS POR PAGAR"),
            ("2101005", "IMPUESTOS POR PAGAR"),
            ("21010050001", "IMPUESTO 15%"),
            ("21010050002", "IMPUESTO EXENTO"),
            ("21010050003", "IMPUESTO 18%"),
            ("2101006", "PROPINAS POR PAGAR"),
            ("210100601", "Maria Salgado"),
            ("2101007", "AFILIADAS"),
            ("2102", "CUOTAS PATRONALES Y RETENCIONES DE IMPUESTOS"),
            ("2102001", "I.H.S.S."),
            ("2102002", "INFOP"),
            ("2102003", "RAP FOSOVI"),
            ("2102004", "HONORARIOS PROFESIONALES"),
            ("2102005", "RETENCION EN LA FUENTE"),
            ("2102006", "IMPUESTO SOBRE LA RENTA"),
            ("2102007", "PRESTAMOS EMPLEADOS POR PAGAR RAP"),
            ("2103", "DOCUMENTOS POR PAGAR"),
            ("2103001", "PAGARES/ LETRAS DE CAMBIO"),
            ("2104", "DOCUMENTOS POR PAGAR COMPAÑIAS Y PARTES RELACIONADAS"),
            ("2104001", "AFILIADAS"),
            ("2105", "INTERESES POR PAGAR"),
            ("2106", "PROVISIONES POR PAGAR"),
            ("2106001", "TRECEAVO MES"),
            ("2106002", "CATORCEAVO MES"),
            ("2106003", "PREAVISO"),
            ("2106004", "CESANTIA"),
            ("2106005", "VACACIONES"),
            ("2107", "DIVIDENDOS POR PAGAR"),
            ("2107001", "SOBRE ACCIONES"),
            ("2108", "DEPOSITOS EN GARANTIA"),
            ("22", "PASIVO FIJO"),
            ("2201", "PRESTAMOS A LARGO PLAZO"),
            ("2201001", "PRESTAMOS BANCARIOS"),
            ("2202", "CANTIDADES PENDIENTES DE APLICACION"),
            ("2202001", "PAGO RECIBIDOS DE CLIENTES"),
            ("23", "PASIVO DIFERIDO"),

            ("3", "PATRIMONIO"),
            ("31", "PATRIMONIO"),
            ("3101", "CAPITAL SOCIAL PAGADO"),
            ("3101001", "CAPITAL SOCIAL PAGADO"),
            ("3102", "CAPITAL SOCIAL NO PAGADO"),
            ("3102001", "CAPITAL SOCIAL NO PAGADO"),
            ("3103", "PAGOS ADICIONALES DE CAPITAL"),
            ("3104", "RESERVA LEGAL"),
            ("3104001", "RESERVA LEGAL"),
            ("3105", "OTRAS RESERVAS DE CAPITAL"),
            ("3106", "SUPERAVIT POR REVALUACION DE ACTIVOS"),
            ("3106001", "REVALUACIONES ACTIVOS"),
            ("3107", "UTILIDADES O PERDIDAS DE EJERCICIO"),
            ("3107001", "UTILIDAD O PERDIDA DEL PERIODO"),
            ("3108", "RESULTADOS ACUMULADOS"),
            ("3108001", "PERDIDA ACUMULADA"),
            ("3108002", "UTILIDAD ACUMULADA"),

            ("4", "INGRESOS"),
            ("41", "INGRESOS POR OPERACIONES"),
            ("4101", "INGRESOS POR OPERACIONES"),
            ("4101001", "INGRESO POR VENTAS"),
            ("410100101", "Camaras"),
            ("410100102", "Desarrollo de Software"),
            ("4101002", "INGRESO POR ALQUILERES"),
            ("4101003", "INGRESOS POR COMISION"),
            ("4101004", "INGRESOS VARIOS"),
            ("4101005", "INGRESO POR SERVICIOS"),
            ("4101006", "DESCUENTO SOBRE VENTA"),
            ("42", "INGRESOS NO OPERACIONALES"),
            ("4201", "INTERESES DEVENGADOS"),
            ("4201001", "INTERESES BANCARIOS DEVENGADOS"),

            ("5", "COSTOS"),
            ("51", "COSTOS OPERATIVOS"),
            ("5101", "COSTOS DE VENTAS"),
            ("5101001", "COSTOS DE VENTA"),
            ("5101002", "DEVOLUCION SOBRE VENTA"),
            ("52", "COMPRAS"),
            ("5201", "COMPRAS PARA VENTAS"),
            ("5201001", "COMPRAS MERCADERIA"),
            ("5201002", "DESCUENTO SOBRE COMPRAS"),

            ("6", "GASTOS"),
            ("61", "GASTOS OPERATIVOS"),
            ("6101", "GASTOS DE VENTA"),
            ("6101001", "SUELDOS Y SALARIOS"),
            ("6101002", "VACACIONES"),
            ("6101003", "BONIFICACIONES"),
            ("6101004", "PRESTACIONES SOCIALES"),
            ("6101005", "TRECEAVO MES"),
            ("6101006", "CATORCEAVO MES"),
            ("6101007", "INFOP"),
            ("6101008", "ENERGIA/ AGUA POTABLE"),
            ("6101009", "SEGURIDAD Y VIGILANCIA"),
            ("6101010", "ALQUILERES DE LOCAL"),
            ("6101011", "MANTENIMIENTO DE AIRES Y OTRO"),
            ("6101012", "ATENCION A EMPLEADOS"),
            ("6101013", "ATENCION A CLIENTES"),
            ("6101014", "PAPELERIA Y UTILES"),
            ("6101015", "IHSS"),
            ("6101016", "RAP FOSOVI"),
            ("6101017", "COMISIONES"),
            ("6101018", "PUBLICIDAD Y MARKETING"),
            ("6101019", "ENLACES, INTERNET Y TELEFONIA"),
            ("6102", "GASTOS POR SERVICIOS"),
            ("6102001", "HONORARIOS PROFESIONALES"),
            ("6103", "IMPUESTOS MUNICIPALES"),
            ("6103001", "IMPUESTOS MUNICIPALES PAGADOS"),
            ("6104", "GASTOS DIVERSOS"),
            ("6104001", "MATERIALES DIVERSOS"),
            ("6105", "PERDIDAS VARIAS"),
            ("6105001", "PERDIDA POR DETERIORO DE EQUIPO"),
            ("6106", "DEPRECIACIONES Y AMORTIZACIONES"),
            ("6106001", "GASTO DEPRECIACION DE EDIFICIOS"),
            ("6106002", "GASTO DEPRECIACION DE MOBILIARIO"),
            ("6106003", "GASTO DEPRECIACION DE VEHICULO"),
            ("6106004", "GASTO DEPRECIACION DE MAQUINARIA"),
            ("6106005", "GASTOS POR AMORTIZACIONES"),
            ("6107", "GASTOS NO DEDUCIBLES"),
            ("6107001", "ISR 25% PAGADO NO DEDUCIBLE"),
            ("6107002", "ACTIVO NETO NO DEDUCIBLE"),
            ("6107003", "TASAS DE SEGURIDAD"),
            ("6107004", "OTROS GASTOS NO DEDUCIBLES"),
            ("62", "GASTOS FINANCIEROS"),
            ("6201", "INTERESES"),
            ("6201001", "INTERESES BANCARIOS"),
            ("6202", "COMISIONES BANCARIAS"),
            ("6202001", "COMISIONES BANCARIAS PAGADAS"),
            ("63", "GASTOS DE ADMINISTRACION"),
            ("6301", "GASTOS DE ADMINISTRACION"),
            ("6301001", "SUELDOS Y SALARIOS"),
            ("6301002", "VACACIONES"),
            ("6301003", "BONIFICACIONES"),
            ("6301004", "PRESTACIONES SOCIALES"),
            ("6301005", "TRECEAVO MES"),
            ("6301006", "CATORCEAVO MES"),
            ("6301007", "INFOP"),
            ("6301008", "ENERGIA/ AGUA POTABLE"),
            ("6301009", "SEGURIDAD Y VIGILANCIA"),
            ("6301010", "ALQUILERES DE LOCAL"),
            ("6301011", "MANTENIMIENTO DE AIRES Y OTRO"),
            ("6301012", "ATENCION A EMPLEADOS"),
            ("6301013", "PAPELERIA Y UTILES"),
            ("6301014", "IHSS"),
            ("6301015", "RAP FOSOVI"),

            ("7", "CUENTAS DE ORDEN"),
            ("71", "DEUDORAS FISCALES"),
            ("7101", "CREDITO FISCAL"),
            ("72", "ACREEDORAS FISCALES"),
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var codes = new HashSet<string>(Accounts.Select(a => a.Code));

            foreach (var (code, name) in Accounts.OrderBy(a => a.Code.Length).ThenBy(a => a.Code))
            {
                var accountType = code[0] - '0';                 // 1..7
                var level = LevelFromLength(code.Length);          // enum AccountLevel int
                var normalSide = NormalSide(accountType);          // 1 Debit / 2 Credit
                var parentCode = ParentCode(code);
                var isLeaf = !codes.Any(c => c != code && c.StartsWith(code));
                var isPostable = isLeaf && code.Length >= 4;       // imputable solo a nivel Mayor o inferior
                var isSystem = code.Length == 1 || code == "3107001";

                var parentClause = parentCode == null
                    ? "NULL"
                    : $"(SELECT Id FROM LedgerAccount WHERE Code = N'{parentCode}')";

                var safeName = name.Replace("'", "''");

                migrationBuilder.Sql(
                    "INSERT INTO LedgerAccount (Code, Name, AccountType, Level, NormalBalanceSide, ParentId, IsPostable, IsActive, IsSystem, Description, ExpenseAccountId, IncomeAccountId, CreationDate, CreatedBy) " +
                    $"VALUES (N'{code}', N'{safeName}', {accountType}, {level}, {normalSide}, {parentClause}, {(isPostable ? 1 : 0)}, 1, {(isSystem ? 1 : 0)}, NULL, NULL, NULL, '2026-01-01T00:00:00', N'system');");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimina solo las cuentas sembradas, hojas primero (código más largo) para respetar la auto-referencia.
            foreach (var (code, _) in Accounts.OrderByDescending(a => a.Code.Length).ThenByDescending(a => a.Code))
            {
                migrationBuilder.Sql($"DELETE FROM LedgerAccount WHERE Code = N'{code}' AND CreatedBy = N'system';");
            }
        }

        private static int LevelFromLength(int len) => len switch
        {
            1 => 1,   // Tipo
            2 => 2,   // Grupo
            4 => 3,   // Mayor
            7 => 4,   // SubCuenta
            _ => 5    // Auxiliar (>7)
        };

        private static int NormalSide(int accountType) => accountType switch
        {
            1 => 1, // Activo  -> Debit
            5 => 1, // Costos  -> Debit
            6 => 1, // Gastos  -> Debit
            7 => 1, // Cuentas de Orden -> Debit (por defecto)
            _ => 2  // Pasivo, Patrimonio, Ingresos -> Credit
        };

        private static string ParentCode(string code) => code.Length switch
        {
            1 => null,
            2 => code.Substring(0, 1),
            4 => code.Substring(0, 2),
            7 => code.Substring(0, 4),
            _ => code.Substring(0, 7)
        };
    }
}
