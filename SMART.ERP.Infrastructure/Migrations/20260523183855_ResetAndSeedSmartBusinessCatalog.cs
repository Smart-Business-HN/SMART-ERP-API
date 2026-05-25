using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <summary>
    /// Reinicia el módulo contable y siembra el catálogo NIIF para PYMES diseñado por el contador
    /// para SMART BUSINESS, S. de R.L. (Honduras). Reemplaza el catálogo Zafra inicial. Borra
    /// asientos/períodos/mapeos previos (operación destructiva — solo BD local). Siembra:
    /// 291 cuentas del nuevo catálogo, 6 centros de costo, configuración (toggle off) y mapeos
    /// AccountingMapping apuntando a los nuevos códigos.
    /// </summary>
    public partial class ResetAndSeedSmartBusinessCatalog : Migration
    {
        private sealed record SeedRow(string Code, string Name, string? Naturaleza, string Fs, bool Postable, bool RequiresCC, bool RequiresT, string? Notes);

        private static readonly SeedRow[] Catalog = new SeedRow[]
        {
            new("1", "ACTIVO", "Deudora", "BG", false, false, false, null),
            new("11", "ACTIVO CORRIENTE", "Deudora", "BG", false, false, false, null),
            new("1101", "EFECTIVO Y EQUIVALENTES DE EFECTIVO", "Deudora", "BG", false, false, false, null),
            new("1101001", "Caja General", "Deudora", "BG", true, false, false, "Efectivo en moneda nacional"),
            new("1101002", "Caja Chica", "Deudora", "BG", true, false, false, "Fondo fijo para gastos menores. Definir monto del fondo en política."),
            new("1101003", "Bancos Moneda Nacional", "Deudora", "BG", false, false, false, "Una auxiliar por cada cuenta bancaria"),
            new("110100301", "BAC Lempiras", "Deudora", "BG", true, false, false, "Cuenta bancaria BAC en HNL"),
            new("1101004", "Bancos Moneda Extranjera", "Deudora", "BG", false, false, false, "Si abren cuenta en USD a futuro. Crear auxiliares como 110100401 BAC USD, etc."),
            new("1101005", "Equivalentes de Efectivo", "Deudora", "BG", true, false, false, "Inversiones temporales de muy alta liquidez (menos de 3 meses)"),
            new("1102", "CUENTAS POR COBRAR COMERCIALES", "Deudora", "BG", false, false, false, "Reestructurado: el detalle de cliente va por dimensión 'Tercero', no por auxiliar"),
            new("1102001", "Clientes Nacionales", "Deudora", "BG", true, false, true, "Reemplaza 1102002. El detalle por cliente se obtiene con la dimensión 'Tercero'."),
            new("1102002", "Clientes del Exterior", "Deudora", "BG", true, false, true, "Por si exportan servicios/software a futuro"),
            new("1102003", "Documentos por Cobrar (pagarés)", "Deudora", "BG", true, false, true, "Antes 1105001"),
            new("1102004", "Anticipos a Proveedores", "Deudora", "BG", true, false, true, "Pagos hechos a proveedores antes de recibir mercadería/servicio"),
            new("1102005", "Préstamos a Empleados", "Deudora", "BG", true, false, true, null),
            new("1102006", "Préstamos a Socios (corto plazo)", "Deudora", "BG", true, false, true, "Cuidado fiscal: el SAR puede asumir intereses presuntos si no se documentan"),
            new("1102007", "Otras Cuentas por Cobrar", "Deudora", "BG", true, false, true, null),
            new("1102008", "(-) Estimación para Cuentas Incobrables", "Acreedora", "BG", true, false, false, "CUENTA CORRECTORA. Política sugerida: estimar 1-3% anual sobre cartera, o por antigüedad"),
            new("1103", "CUENTAS POR COBRAR PARTES RELACIONADAS", "Deudora", "BG", false, false, false, null),
            new("1103001", "Cuentas por Cobrar Afiliadas", "Deudora", "BG", true, false, true, null),
            new("1103002", "Cuentas por Cobrar Socios", "Deudora", "BG", true, false, true, "Separa préstamos comerciales de socios para mejor control"),
            new("1104", "INVENTARIOS", "Deudora", "BG", false, false, false, "Reestructurado por categoría real. Eliminadas cuentas de 'Materia Prima' y 'Producción en Proceso' que no aplican (no fabrican)"),
            new("1104001", "Inventario CCTV", "Deudora", "BG", false, false, false, "Equipo de videovigilancia para venta"),
            new("110400101", "Hikvision", "Deudora", "BG", true, false, false, null),
            new("110400102", "Dahua", "Deudora", "BG", true, false, false, null),
            new("110400103", "EPCOM", "Deudora", "BG", true, false, false, null),
            new("110400104", "Ubiquiti UniFi Protect / CCTV", "Deudora", "BG", true, false, false, null),
            new("110400105", "Otras marcas CCTV", "Deudora", "BG", true, false, false, null),
            new("1104002", "Inventario Networking - Equipos", "Deudora", "BG", false, false, false, "Routers, switches, AP, antenas"),
            new("110400201", "Ubiquiti Networking", "Deudora", "BG", true, false, false, null),
            new("110400202", "MikroTik", "Deudora", "BG", true, false, false, null),
            new("110400203", "Otras marcas Networking", "Deudora", "BG", true, false, false, null),
            new("1104003", "Inventario Materiales de Cableado", "Deudora", "BG", false, false, false, "Materiales consumibles de proyectos"),
            new("110400301", "Cable UTP / Fibra óptica", "Deudora", "BG", true, false, false, null),
            new("110400302", "Conectores, jacks y patch cords", "Deudora", "BG", true, false, false, null),
            new("110400303", "Racks, paneles y organizadores", "Deudora", "BG", true, false, false, null),
            new("110400304", "Canaletas, tubería y accesorios", "Deudora", "BG", true, false, false, null),
            new("1104004", "Mercadería en Tránsito", "Deudora", "BG", true, false, true, "Importaciones embarcadas no recibidas. Acumula costo + flete + seguro + DAI hasta nacionalizar"),
            new("1104005", "(-) Estimación para Obsolescencia", "Acreedora", "BG", true, false, false, "CUENTA CORRECTORA. Para inventario con baja rotación o tecnología desfasada"),
            new("1105", "PAGOS ANTICIPADOS", "Deudora", "BG", false, false, false, "Consolida lo que estaba duplicado en 1110, 1301 y 1303"),
            new("1105001", "Primas de Seguros pagadas por anticipado", "Deudora", "BG", true, false, true, null),
            new("1105002", "Alquileres pagados por anticipado", "Deudora", "BG", true, false, true, null),
            new("1105003", "Suscripciones y Licencias prepagadas", "Deudora", "BG", true, false, true, "GitHub anual, Microsoft 365 anual, antivirus, etc."),
            new("1105004", "Otros Pagos Anticipados", "Deudora", "BG", true, false, true, null),
            new("1106", "CRÉDITOS FISCALES E IMPUESTOS POR ANTICIPADO", "Deudora", "BG", false, false, false, "Antes 1302. Reestructurado con nombres correctos"),
            new("1106001", "ISV Crédito Fiscal - Compras Locales 15%", "Deudora", "BG", true, false, true, "Ver Hoja 'Cuentas Clave - Notas' para prorrateo"),
            new("1106002", "ISV Crédito Fiscal - Importaciones (Art. 8)", "Deudora", "BG", true, false, true, "ISV pagado en aduana sobre importaciones"),
            new("1106003", "Retención ISR 1% (que nos retienen)", "Deudora", "BG", true, false, true, "Retenciones que clientes nos hicieron, deducibles del ISR anual"),
            new("1106004", "Retención ISR 12.5% sobre Honorarios (que nos retienen)", "Deudora", "BG", true, false, true, "Antes mal nombrada como 15%. La tarifa real es 12.5%"),
            new("1106005", "Pagos a Cuenta ISR (cuotas trimestrales)", "Deudora", "BG", true, false, false, "Pagos a cuenta del ISR estimado (junio, septiembre, diciembre)"),
            new("1106006", "Crédito Fiscal por Ventas con Tarjeta de Crédito", "Deudora", "BG", true, false, true, null),
            new("1107", "OTROS ACTIVOS CORRIENTES", "Deudora", "BG", true, false, true, null),
            new("12", "ACTIVO NO CORRIENTE", "Deudora", "BG", false, false, false, null),
            new("1201", "PROPIEDAD, PLANTA Y EQUIPO (COSTO)", "Deudora", "BG", false, false, false, "Separados por tipo de activo para mejor control de depreciación"),
            new("1201001", "Terrenos", "Deudora", "BG", true, false, false, "No se deprecia"),
            new("1201002", "Edificios", "Deudora", "BG", true, false, false, "Vida útil sugerida: 40 años (2.5% anual). SAR acepta 2.5%"),
            new("1201003", "Mobiliario y Equipo de Oficina", "Deudora", "BG", true, false, false, "Vida útil: 10 años (10% anual)"),
            new("1201004", "Equipo de Cómputo y Servidores", "Deudora", "BG", true, false, false, "Laptops, servidores, monitores. Vida útil: 3-5 años (20-33% anual)"),
            new("1201005", "Equipo de Comunicaciones (uso interno)", "Deudora", "BG", true, false, false, "Routers, switches, APs usados internamente. Vida útil: 5 años (20%)"),
            new("1201006", "Herramientas de Instalación", "Deudora", "BG", true, false, false, "Crimpadoras, fusionadoras de fibra, testers, escaleras. Vida útil: 5 años"),
            new("1201007", "Vehículos", "Deudora", "BG", true, false, false, "Vida útil: 5 años (20% anual)"),
            new("1202", "(-) DEPRECIACIÓN ACUMULADA", "Acreedora", "BG", false, false, false, "Una contracuenta por cada tipo de activo"),
            new("1202001", "(-) Dep. Acum. Edificios", "Acreedora", "BG", true, false, false, null),
            new("1202002", "(-) Dep. Acum. Mobiliario y Eq. Oficina", "Acreedora", "BG", true, false, false, null),
            new("1202003", "(-) Dep. Acum. Equipo de Cómputo", "Acreedora", "BG", true, false, false, null),
            new("1202004", "(-) Dep. Acum. Equipo de Comunicaciones", "Acreedora", "BG", true, false, false, null),
            new("1202005", "(-) Dep. Acum. Herramientas", "Acreedora", "BG", true, false, false, null),
            new("1202006", "(-) Dep. Acum. Vehículos", "Acreedora", "BG", true, false, false, null),
            new("1203", "ACTIVOS INTANGIBLES", "Deudora", "BG", false, false, false, "Ampliado para SaaS"),
            new("1203001", "Software Desarrollado (Ventix)", "Deudora", "BG", true, false, false, "Capitalización opcional bajo NIIF PYMES Sec. 18. Ver Hoja 'Cuentas Clave - Notas'"),
            new("1203002", "Licencias de Software Adquiridas", "Deudora", "BG", true, false, true, "Licencias perpetuas o multi-año (Office, Adobe, etc.)"),
            new("1203003", "Dominios y Sitios Web", "Deudora", "BG", true, false, false, "Costos significativos de desarrollo de sitio web propio"),
            new("1203004", "Marcas y Patentes", "Deudora", "BG", true, false, false, "Si registran la marca Ventix u otras"),
            new("1204", "(-) AMORTIZACIÓN ACUMULADA INTANGIBLES", "Acreedora", "BG", false, false, false, null),
            new("1204001", "(-) Amort. Acum. Software Desarrollado", "Acreedora", "BG", true, false, false, "Vida útil sugerida: 3-5 años"),
            new("1204002", "(-) Amort. Acum. Licencias", "Acreedora", "BG", true, false, false, "Según vigencia de la licencia"),
            new("1204003", "(-) Amort. Acum. Otros Intangibles", "Acreedora", "BG", true, false, false, null),
            new("1205", "INVERSIONES PERMANENTES", "Deudora", "BG", false, false, false, null),
            new("1205001", "Inversiones en Afiliadas", "Deudora", "BG", true, false, true, null),
            new("1205002", "Inversiones en Asociadas", "Deudora", "BG", true, false, true, null),
            new("1206", "DEPÓSITOS EN GARANTÍA", "Deudora", "BG", false, false, false, null),
            new("1206001", "Depósitos por Alquiler", "Deudora", "BG", true, false, true, "Garantías depositadas al arrendador"),
            new("1206002", "Otros Depósitos en Garantía", "Deudora", "BG", true, false, true, null),
            new("1207", "PRÉSTAMOS A SOCIOS LARGO PLAZO", "Deudora", "BG", true, false, true, null),
            new("2", "PASIVO", "Acreedora", "BG", false, false, false, null),
            new("21", "PASIVO CORRIENTE", "Acreedora", "BG", false, false, false, null),
            new("2101", "CUENTAS POR PAGAR COMERCIALES", "Acreedora", "BG", false, false, false, "Solo proveedores REALES. Comercios para gastos no van como auxiliares"),
            new("2101001", "Proveedores Nacionales", "Acreedora", "BG", true, false, true, "Solo proveedores con quienes hay relación comercial recurrente. El tercero se gestiona por dimensión"),
            new("2101002", "Proveedores del Exterior", "Acreedora", "BG", true, false, true, "Para importaciones directas"),
            new("2101003", "Acreedores Varios", "Acreedora", "BG", true, false, true, "Para terceros no proveedores con cuentas pendientes"),
            new("2101004", "Otras Cuentas por Pagar", "Acreedora", "BG", true, false, true, null),
            new("2102", "TARJETAS DE CRÉDITO POR PAGAR", "Acreedora", "BG", false, false, false, "ANTES estaban como [ARCHIVED] proveedores. Es un pasivo financiero, NO un proveedor"),
            new("2102001", "BAC Credomatic TC 2086", "Acreedora", "BG", true, false, false, null),
            new("2102002", "BAC Credomatic TC 5386", "Acreedora", "BG", true, false, false, null),
            new("2102003", "Ficohsa TC 7366", "Acreedora", "BG", true, false, false, null),
            new("2102004", "Ficohsa TC 8478", "Acreedora", "BG", true, false, false, null),
            new("2102005", "Davivienda TC 5969", "Acreedora", "BG", true, false, false, null),
            new("2102006", "Banpais TC 1112", "Acreedora", "BG", true, false, false, null),
            new("2102007", "Cuscatlán TC", "Acreedora", "BG", true, false, false, null),
            new("2102008", "Banrural TC 1093", "Acreedora", "BG", true, false, false, null),
            new("2103", "IMPUESTOS POR PAGAR", "Acreedora", "BG", false, false, false, "Reestructurado con separación clara de ISV"),
            new("2103001", "ISV Débito Fiscal 15% (de ventas)", "Acreedora", "BG", true, false, true, "Antes 21010050001. Es el ISV que cobramos al cliente. Se reclasifica a 2103003 al cierre del mes"),
            new("2103002", "ISV Débito Fiscal 18% (de ventas)", "Acreedora", "BG", true, false, true, "Solo si llegan a vender productos específicos al 18%. Probablemente no aplica"),
            new("2103003", "ISV por Pagar (neto del período)", "Acreedora", "BG", true, false, false, "Resultado mensual: ISV débito − ISV crédito acreditable. Ver Hoja 'Cuentas Clave - Notas'"),
            new("2103004", "ISR del Período por Pagar", "Acreedora", "BG", true, false, false, "Provisión mensual del ISR 25% sobre utilidad estimada"),
            new("2103005", "Aportación Solidaria por Pagar", "Acreedora", "BG", true, false, false, "5% adicional sobre utilidad mayor a L. 1,000,000"),
            new("2103006", "Impuesto al Activo Neto por Pagar", "Acreedora", "BG", true, false, false, "1% sobre activo neto (acreditable contra ISR)"),
            new("2103007", "Impuesto Municipal por Pagar", "Acreedora", "BG", true, false, false, "Permiso de operación municipal anual"),
            new("2103008", "Tasa de Seguridad Poblacional por Pagar", "Acreedora", "BG", true, false, false, "Si aplica según facturación"),
            new("2104", "RETENCIONES POR PAGAR (que retenemos a terceros)", "Acreedora", "BG", false, false, false, "Antes 2102. Más granular"),
            new("2104001", "Retención ISR 12.5% Honorarios Profesionales", "Acreedora", "BG", true, false, true, "Cuando pagamos a profesionales independientes"),
            new("2104002", "Retención ISR 1% sobre Bienes y Servicios", "Acreedora", "BG", true, false, true, "Si nos clasifican como agente retenedor"),
            new("2104003", "Retención ISV 15% (Art. 13)", "Acreedora", "BG", true, false, true, "Solo si Smart Business es designado retenedor de ISV"),
            new("2104004", "Retención ISR Planilla Empleados", "Acreedora", "BG", true, false, true, "Cuando empleados ganen sobre el mínimo gravable"),
            new("2104005", "Aporte Empleado IHSS", "Acreedora", "BG", true, false, true, "Retención al empleado (3.5% sobre tope)"),
            new("2104006", "Aporte Empleado RAP", "Acreedora", "BG", true, false, true, "Retención al empleado (1.5% sobre exceso de tope IHSS)"),
            new("2105", "CUOTAS PATRONALES POR PAGAR", "Acreedora", "BG", false, false, false, "Aportes del empleador, separados del aporte del empleado"),
            new("2105001", "IHSS Patronal (EM + IVM)", "Acreedora", "BG", true, false, false, "Aprox. 7.83% sobre salarios (EM 5%+IVM 2.83%)"),
            new("2105002", "RAP Patronal (1.5%)", "Acreedora", "BG", true, false, false, "1.5% sobre exceso del tope IHSS"),
            new("2105003", "INFOP Patronal (1%)", "Acreedora", "BG", true, false, false, "1% sobre planilla bruta"),
            new("2106", "SUELDOS Y PRESTACIONES POR PAGAR", "Acreedora", "BG", false, false, false, null),
            new("2106001", "Sueldos por Pagar", "Acreedora", "BG", true, false, true, null),
            new("2106002", "Bonos y Comisiones por Pagar", "Acreedora", "BG", true, false, true, null),
            new("2106003", "Honorarios por Pagar (terceros)", "Acreedora", "BG", true, false, true, "Antes 2102004"),
            new("2107", "PROVISIONES LABORALES", "Acreedora", "BG", false, false, false, "Antes 2106. Las provisiones laborales acumuladas al cierre de cada mes"),
            new("2107001", "Provisión Treceavo Mes (Aguinaldo)", "Acreedora", "BG", true, false, true, "1/12 del sueldo mensual por empleado por mes"),
            new("2107002", "Provisión Catorceavo Mes", "Acreedora", "BG", true, false, true, "1/12 del sueldo mensual por empleado por mes"),
            new("2107003", "Provisión Vacaciones", "Acreedora", "BG", true, false, true, null),
            new("2107004", "Provisión Preaviso y Cesantía", "Acreedora", "BG", true, false, true, "Pasivo contingente. NIIF PYMES Sec. 21"),
            new("2108", "INGRESOS DIFERIDOS Y ANTICIPOS DE CLIENTES", "Acreedora", "BG", false, false, false, "Para SaaS y proyectos"),
            new("2108001", "Ingresos Diferidos por Suscripciones SaaS", "Acreedora", "BG", true, true, true, "Suscripciones Ventix anuales cobradas. Hoy vacío (cliente paga mensual)"),
            new("2108002", "Anticipos de Clientes por Proyectos", "Acreedora", "BG", true, true, true, "Anticipos recibidos en proyectos de cableado antes de reconocer ingreso"),
            new("2108003", "Otros Ingresos Cobrados por Anticipado", "Acreedora", "BG", true, true, true, null),
            new("2109", "DOCUMENTOS POR PAGAR (pagarés)", "Acreedora", "BG", true, false, true, "Antes 2103"),
            new("2110", "INTERESES POR PAGAR", "Acreedora", "BG", true, false, true, "Intereses devengados no pagados de préstamos"),
            new("2111", "DIVIDENDOS POR PAGAR", "Acreedora", "BG", true, false, true, "Distribuciones de utilidad decretadas a los socios pendientes de pagar"),
            new("22", "PASIVO NO CORRIENTE", "Acreedora", "BG", false, false, false, null),
            new("2201", "PRÉSTAMOS BANCARIOS A LARGO PLAZO", "Acreedora", "BG", true, false, true, null),
            new("2202", "DOCUMENTOS POR PAGAR A LARGO PLAZO", "Acreedora", "BG", true, false, true, null),
            new("2203", "CUENTAS POR PAGAR PARTES RELACIONADAS", "Acreedora", "BG", true, false, true, null),
            new("3", "PATRIMONIO", "Acreedora", "BG", false, false, false, null),
            new("31", "PATRIMONIO", "Acreedora", "BG", false, false, false, null),
            new("3101", "Capital Social Pagado", "Acreedora", "BG", true, false, true, "Aporte de los socios efectivamente pagado, según escritura"),
            new("3102", "(-) Capital Social Suscrito No Pagado", "Deudora", "BG", true, false, true, "Capital suscrito pendiente de pago por los socios"),
            new("3103", "Aportaciones para Futuras Capitalizaciones", "Acreedora", "BG", true, false, true, "Aportes de socios pendientes de formalizar como aumento de capital"),
            new("3104", "Reserva Legal", "Acreedora", "BG", true, false, false, "5% de la utilidad anual hasta acumular 20% del capital social (Cód. de Comercio)"),
            new("3105", "Otras Reservas", "Acreedora", "BG", true, false, false, null),
            new("3106", "Superávit por Revaluación de Activos", "Acreedora", "BG", true, false, false, null),
            new("3107", "Utilidades / Pérdidas Acumuladas", "Acreedora", "BG", false, false, false, null),
            new("3107001", "Utilidades Acumuladas", "Acreedora", "BG", true, false, false, null),
            new("3107002", "Pérdidas Acumuladas", "Deudora", "BG", true, false, false, null),
            new("3107003", "Ajuste por Reconstrucción de Saldos Iniciales", "Acreedora", "BG", true, false, false, "CRÍTICA: el diferencial del balance de apertura al 01/05/2026 va aquí mientras se documenta históricamente"),
            new("3108", "Utilidad o Pérdida del Ejercicio", "Acreedora", "BG", true, false, false, "Se reclasifica a 3107 al cierre anual"),
            new("3109", "Dividendos Decretados (corrige patrimonio)", "Deudora", "BG", true, false, false, "Cuenta correctora de utilidades cuando se decreta distribución"),
            new("4", "INGRESOS", "Acreedora", "ER", false, false, false, null),
            new("41", "INGRESOS OPERATIVOS", "Acreedora", "ER", false, false, false, null),
            new("4101", "INGRESOS POR VENTAS DE MERCADERÍA", "Acreedora", "ER", false, false, false, "Una sola cuenta segmentada por Centro de Costo. Eliminados los auxiliares por cliente"),
            new("4101001", "Ingresos por Ventas", "Acreedora", "ER", true, true, true, "Cliente va por dimensión 'Tercero'. Línea de negocio va por dimensión 'Centro de Costo'"),
            new("4102", "INGRESOS POR PROYECTOS", "Acreedora", "ER", false, false, false, "Para proyectos de cableado, instalación, integración"),
            new("4102001", "Ingresos por Proyectos de Cableado e Instalación", "Acreedora", "ER", true, true, true, "Reconocer por avance de obra (NIIF PYMES Sec. 23) si el proyecto cruza meses"),
            new("4103", "INGRESOS POR SERVICIOS SOFTWARE (SaaS)", "Acreedora", "ER", false, false, false, null),
            new("4103001", "Ingresos por Suscripciones SaaS - Ventix", "Acreedora", "ER", true, true, true, "Reconocimiento mensual. Si pagan anual prepagado, primero va a 2108001"),
            new("4103002", "Ingresos por Setup / Implementación SaaS", "Acreedora", "ER", true, true, true, null),
            new("4104", "INGRESOS POR SERVICIOS PROFESIONALES", "Acreedora", "ER", false, false, false, null),
            new("4104001", "Ingresos por Desarrollo a la Medida", "Acreedora", "ER", true, true, true, null),
            new("4104002", "Ingresos por Consultoría y Asesoría", "Acreedora", "ER", true, true, true, null),
            new("4104003", "Ingresos por Soporte Técnico", "Acreedora", "ER", true, true, true, null),
            new("4105", "INGRESOS POR ALQUILERES", "Acreedora", "ER", true, true, true, null),
            new("4106", "INGRESOS POR COMISIONES", "Acreedora", "ER", true, true, true, null),
            new("4107", "INGRESOS VARIOS OPERATIVOS", "Acreedora", "ER", true, true, true, null),
            new("4108", "(-) DEVOLUCIONES SOBRE VENTAS", "Deudora", "ER", true, true, true, "Cuenta correctora de ingresos"),
            new("4109", "(-) DESCUENTOS Y REBAJAS SOBRE VENTAS", "Deudora", "ER", true, true, true, "Cuenta correctora de ingresos"),
            new("42", "INGRESOS NO OPERATIVOS", "Acreedora", "ER", false, false, false, null),
            new("4201", "Intereses Bancarios Devengados", "Acreedora", "ER", true, false, false, null),
            new("4202", "Diferencial Cambiario (ganancia)", "Acreedora", "ER", true, false, false, "Por revaluación de cuentas en USD si llegan a tenerlas"),
            new("4203", "Ganancia en Venta de Activo Fijo", "Acreedora", "ER", true, false, false, null),
            new("4204", "Otros Ingresos No Operativos", "Acreedora", "ER", true, false, true, null),
            new("5", "COSTOS", "Deudora", "ER", false, false, false, null),
            new("51", "COSTOS DE VENTAS Y SERVICIOS", "Deudora", "ER", false, false, false, null),
            new("5101", "COSTO DE VENTA DE MERCADERÍA", "Deudora", "ER", true, true, false, "Costo de equipo CCTV y networking vendido. Segmentado por Centro de Costo"),
            new("5102", "COSTO DE PROYECTOS DE CABLEADO", "Deudora", "ER", false, false, false, "Para proyectos de cableado estructurado"),
            new("5102001", "Costo de Proyectos - Materiales", "Deudora", "ER", true, true, true, "Cable, conectores, racks consumidos en el proyecto"),
            new("5102002", "Costo de Proyectos - Subcontratación Técnica", "Deudora", "ER", true, true, true, "CRÍTICO. Honorarios pagados a técnicos subcontratados. Con retención 12.5% si aplica"),
            new("5102003", "Costo de Proyectos - Equipo Instalado", "Deudora", "ER", true, true, true, "Equipo del inventario que se instala en el proyecto"),
            new("5102004", "Costo de Proyectos - Otros (transporte, viáticos)", "Deudora", "ER", true, true, true, null),
            new("5103", "COSTO DE SERVICIOS SOFTWARE (SaaS)", "Deudora", "ER", false, false, false, null),
            new("5103001", "Hosting e Infraestructura Cloud (Ventix)", "Deudora", "ER", true, true, true, "Dokploy, AWS, dominios productivos, CDN"),
            new("5103002", "Comisiones de Pasarela de Pago", "Deudora", "ER", true, true, true, "% de comisión cobrado por la pasarela sobre suscripciones"),
            new("5103003", "Servicios de Terceros Software", "Deudora", "ER", true, true, true, "APIs de pago, SMS, email transaccional"),
            new("5104", "(-) DEVOLUCIONES SOBRE COMPRAS", "Acreedora", "ER", true, true, true, "Cuenta correctora de costo"),
            new("5105", "(-) DESCUENTOS SOBRE COMPRAS", "Acreedora", "ER", true, true, true, "Cuenta correctora de costo"),
            new("6", "GASTOS", "Deudora", "ER", false, false, false, null),
            new("61", "GASTOS DE OPERACIÓN", "Deudora", "ER", false, false, false, "Consolidados administración y ventas en un solo grupo segmentado por Centro de Costo"),
            new("6101", "SUELDOS Y PRESTACIONES", "Deudora", "ER", false, false, false, null),
            new("6101001", "Sueldos y Salarios", "Deudora", "ER", true, true, true, null),
            new("6101002", "Bonificaciones e Incentivos", "Deudora", "ER", true, true, true, null),
            new("6101003", "Vacaciones", "Deudora", "ER", true, true, true, null),
            new("6101004", "Treceavo Mes (Aguinaldo)", "Deudora", "ER", true, true, true, null),
            new("6101005", "Catorceavo Mes", "Deudora", "ER", true, true, true, null),
            new("6101006", "IHSS Patronal", "Deudora", "ER", true, true, true, null),
            new("6101007", "RAP Patronal", "Deudora", "ER", true, true, true, null),
            new("6101008", "INFOP Patronal", "Deudora", "ER", true, true, true, null),
            new("6101009", "Preaviso y Cesantía (provisión gasto)", "Deudora", "ER", true, true, true, null),
            new("6101010", "Atenciones y Beneficios a Empleados", "Deudora", "ER", true, true, true, "Alimentación, regalos, etc. Cuidado con deducibilidad"),
            new("6101011", "Capacitación", "Deudora", "ER", true, true, true, "Cursos, certificaciones técnicas"),
            new("6101012", "Uniformes y Equipo de Protección", "Deudora", "ER", true, true, true, null),
            new("6102", "SERVICIOS PROFESIONALES", "Deudora", "ER", false, false, false, null),
            new("6102001", "Honorarios Contables y Fiscales", "Deudora", "ER", true, true, true, null),
            new("6102002", "Honorarios Legales", "Deudora", "ER", true, true, true, null),
            new("6102003", "Honorarios Auditoría", "Deudora", "ER", true, true, true, null),
            new("6102004", "Otros Honorarios Profesionales", "Deudora", "ER", true, true, true, "OJO: para subcontratación TÉCNICA de proyectos usar 5102002, no esta cuenta"),
            new("6103", "ALQUILERES Y SERVICIOS BÁSICOS", "Deudora", "ER", false, false, false, null),
            new("6103001", "Alquileres de Local / Oficina", "Deudora", "ER", true, true, true, null),
            new("6103002", "Energía Eléctrica", "Deudora", "ER", true, true, true, null),
            new("6103003", "Agua y Servicios Sanitarios", "Deudora", "ER", true, true, true, null),
            new("6103004", "Internet y Telecomunicaciones", "Deudora", "ER", true, true, true, "Internet de oficina, telefonía"),
            new("6104", "GASTOS DE TECNOLOGÍA", "Deudora", "ER", false, false, false, "GRUPO NUEVO crítico para empresa tech"),
            new("6104001", "Hosting y Cloud Uso Interno", "Deudora", "ER", true, true, true, "Servidor de prueba, infraestructura no productiva"),
            new("6104002", "Licencias de Software de Desarrollo", "Deudora", "ER", true, true, true, "GitHub Copilot, IDEs, herramientas dev"),
            new("6104003", "Suscripciones SaaS de Apoyo", "Deudora", "ER", true, true, true, "Microsoft 365, Google Workspace, Notion, Slack, etc."),
            new("6104004", "Servicios de Diseño y Marketing Digital", "Deudora", "ER", true, true, true, "Canva, Figma, Adobe Creative Cloud"),
            new("6104005", "Antivirus, Backup y Seguridad", "Deudora", "ER", true, true, true, null),
            new("6105", "PUBLICIDAD Y MARKETING", "Deudora", "ER", false, false, false, null),
            new("6105001", "Publicidad Digital (Facebook Ads, Google Ads)", "Deudora", "ER", true, true, true, null),
            new("6105002", "Material Promocional Impreso", "Deudora", "ER", true, true, true, null),
            new("6105003", "Eventos y Activaciones", "Deudora", "ER", true, true, true, null),
            new("6105004", "Atención a Clientes", "Deudora", "ER", true, true, true, null),
            new("6106", "GASTOS DE VIAJES Y TRANSPORTE", "Deudora", "ER", false, false, false, "Importante para proyectos en sitio"),
            new("6106001", "Viáticos Nacionales", "Deudora", "ER", true, true, true, null),
            new("6106002", "Combustible", "Deudora", "ER", true, true, true, null),
            new("6106003", "Peajes y Estacionamientos", "Deudora", "ER", true, true, true, null),
            new("6106004", "Transporte Aéreo y Hospedaje", "Deudora", "ER", true, true, true, null),
            new("6107", "GASTOS DE OFICINA", "Deudora", "ER", false, false, false, null),
            new("6107001", "Papelería y Útiles", "Deudora", "ER", true, true, true, null),
            new("6107002", "Cafetería y Limpieza", "Deudora", "ER", true, true, true, null),
            new("6107003", "Seguridad y Vigilancia", "Deudora", "ER", true, true, true, null),
            new("6107004", "Mantenimiento de Oficina", "Deudora", "ER", true, true, true, null),
            new("6108", "MANTENIMIENTO Y REPARACIONES", "Deudora", "ER", false, false, false, null),
            new("6108001", "Mantenimiento Equipo de Cómputo", "Deudora", "ER", true, true, true, null),
            new("6108002", "Mantenimiento de Vehículos", "Deudora", "ER", true, true, true, null),
            new("6108003", "Mantenimiento Aires y Otros", "Deudora", "ER", true, true, true, null),
            new("6109", "SEGUROS", "Deudora", "ER", false, false, false, null),
            new("6109001", "Seguros de Vehículos", "Deudora", "ER", true, true, true, null),
            new("6109002", "Seguros de Equipo y Mercadería", "Deudora", "ER", true, true, true, null),
            new("6109003", "Otros Seguros", "Deudora", "ER", true, true, true, null),
            new("6110", "IMPUESTOS Y TASAS (deducibles)", "Deudora", "ER", false, false, false, null),
            new("6110001", "Impuesto Municipal Pagado", "Deudora", "ER", true, true, false, null),
            new("6110002", "Permisos y Licencias", "Deudora", "ER", true, true, false, null),
            new("6110003", "Tasa de Bomberos", "Deudora", "ER", true, true, false, null),
            new("6111", "DEPRECIACIONES Y AMORTIZACIONES", "Deudora", "ER", false, false, false, null),
            new("6111001", "Gasto Depreciación Edificios", "Deudora", "ER", true, true, false, null),
            new("6111002", "Gasto Depreciación Mobiliario y Eq. Oficina", "Deudora", "ER", true, true, false, null),
            new("6111003", "Gasto Depreciación Equipo de Cómputo", "Deudora", "ER", true, true, false, null),
            new("6111004", "Gasto Depreciación Equipo de Comunicaciones", "Deudora", "ER", true, true, false, null),
            new("6111005", "Gasto Depreciación Herramientas", "Deudora", "ER", true, true, false, null),
            new("6111006", "Gasto Depreciación Vehículos", "Deudora", "ER", true, true, false, null),
            new("6111007", "Gasto Amortización Software Desarrollado", "Deudora", "ER", true, true, false, "Amortización de Ventix capitalizado"),
            new("6111008", "Gasto Amortización Licencias", "Deudora", "ER", true, true, false, null),
            new("6111009", "Gasto Amortización Otros Intangibles", "Deudora", "ER", true, true, false, null),
            new("6112", "ESTIMACIONES Y DETERIOROS", "Deudora", "ER", false, false, false, null),
            new("6112001", "Estimación para Cuentas Incobrables", "Deudora", "ER", true, true, false, "Contra-cuenta de 1102008"),
            new("6112002", "Estimación para Obsolescencia de Inventario", "Deudora", "ER", true, true, false, "Contra-cuenta de 1104005"),
            new("6112003", "Pérdida por Deterioro de Activos", "Deudora", "ER", true, true, false, null),
            new("6113", "GASTOS DIVERSOS DEDUCIBLES", "Deudora", "ER", true, true, true, null),
            new("62", "GASTOS FINANCIEROS", "Deudora", "ER", false, false, false, null),
            new("6201", "Intereses Bancarios Pagados", "Deudora", "ER", true, false, true, null),
            new("6202", "Intereses Tarjetas de Crédito", "Deudora", "ER", true, false, true, "Separado para visibilidad del costo de tarjetas"),
            new("6203", "Comisiones Bancarias", "Deudora", "ER", true, false, true, null),
            new("6204", "Diferencial Cambiario (pérdida)", "Deudora", "ER", true, false, false, null),
            new("6205", "Otros Gastos Financieros", "Deudora", "ER", true, false, true, null),
            new("63", "GASTOS NO DEDUCIBLES", "Deudora", "ER", false, false, false, "Gastos contables que el SAR NO acepta como deducibles. Se reflejan al final del ER y se conciliarán para el ISR"),
            new("6301", "ISR Pagado No Deducible", "Deudora", "ER", true, false, false, null),
            new("6302", "Impuesto al Activo Neto No Deducible", "Deudora", "ER", true, false, false, null),
            new("6303", "Tasa de Seguridad No Deducible", "Deudora", "ER", true, false, false, null),
            new("6304", "Multas y Recargos", "Deudora", "ER", true, false, true, null),
            new("6305", "ISV No Acreditable (por prorrateo)", "Deudora", "ER", true, true, false, "CRÍTICO. Por las ventas exentas y exoneradas. Ver Hoja 'Cuentas Clave - Notas'"),
            new("6306", "Otros Gastos No Deducibles", "Deudora", "ER", true, true, true, null),
            new("64", "IMPUESTO SOBRE LA RENTA", "Deudora", "ER", false, false, false, "Para reflejar el gasto del ISR del período"),
            new("6401", "Gasto ISR del Período (25%)", "Deudora", "ER", true, false, false, "Contracuenta de 2103004"),
            new("6402", "Gasto Aportación Solidaria (5%)", "Deudora", "ER", true, false, false, "Si aplica"),
            new("7", "CUENTAS DE ORDEN", null, "Memorando", false, false, false, "Información memorando, sin impacto en BG o ER"),
            new("71", "DEUDORAS DE ORDEN", "Deudora", "Memorando", false, false, false, null),
            new("7101", "Activos Totalmente Depreciados en Uso", "Deudora", "Memorando", true, false, false, null),
            new("7102", "Mercadería en Consignación Recibida", "Deudora", "Memorando", true, false, true, null),
            new("7103", "Equipo en Demostración", "Deudora", "Memorando", true, false, true, null),
            new("7104", "Garantías Recibidas", "Deudora", "Memorando", true, false, true, null),
            new("72", "ACREEDORAS DE ORDEN (contracuentas)", "Acreedora", "Memorando", false, false, false, null),
            new("7201", "Contracuenta Activos Depreciados en Uso", "Acreedora", "Memorando", true, false, false, null),
            new("7202", "Contracuenta Mercadería en Consignación", "Acreedora", "Memorando", true, false, true, null),
            new("7203", "Contracuenta Equipo en Demostración", "Acreedora", "Memorando", true, false, true, null),
            new("7204", "Contracuenta Garantías Recibidas", "Acreedora", "Memorando", true, false, true, null),
        };

        private static readonly (string Code, string Name, string Description)[] CostCenters = new[]
        {
            ("01", "CCTV - Distribución", "Venta de equipo de videovigilancia (Hikvision, Dahua, Ubiquiti, EPCOM)"),
            ("02", "Networking - Venta de Equipo", "Venta de equipo de redes (routers, switches, AP, antenas)"),
            ("03", "Networking - Proyectos Cableado", "Proyectos de cableado estructurado e instalación (servicios)"),
            ("04", "Software - Ventix SaaS", "Suscripciones recurrentes de Ventix"),
            ("05", "Software - Servicios Profesionales", "Desarrollo a la medida, consultoría, soporte fuera de Ventix"),
            ("99", "General / Administración", "Gastos no atribuibles a una línea específica"),
        };

        // (AccountMappingKey int) -> código nuevo del catálogo
        private static readonly (int Key, string Code)[] Mappings = new[]
        {
            (1,  "1102001"),   // AccountsReceivable
            (2,  "4101001"),   // SalesRevenue
            (3,  "2103001"),   // SalesTaxPayable15
            (4,  "2103002"),   // SalesTaxPayable18
            (5,  "5101"),      // CostOfGoodsSold (mayor con movimiento)
            (6,  "1104001"),   // Inventory (por defecto CCTV; ajustable en Configuración Contable)
            (7,  "2101001"),   // AccountsPayable
            (8,  "1106001"),   // InputTaxCredit15
            (9,  "1106002"),   // InputTaxCredit18 (importaciones)
            (10, "1101001"),   // CashOnHand
            (11, "4204"),      // InventoryAdjustmentIncrease (otros ingresos no operativos)
            (12, "6112002"),   // InventoryAdjustmentDecrease (estimación obsolescencia gasto)
            (13, "3107003"),   // OpeningEquity (ajuste por reconstrucción de saldos)
            (14, "6113"),      // DefaultExpense (gastos diversos deducibles)
        };

        protected override void Up(MigrationBuilder mb)
        {
            // ===== 1) Wipe contable previo (orden por FKs) =====
            mb.Sql("DELETE FROM JournalEntryLine;");
            mb.Sql("UPDATE JournalEntry SET ReversesJournalEntryId = NULL, ReversedByJournalEntryId = NULL;");
            mb.Sql("UPDATE FiscalYear SET ClosingJournalEntryId = NULL;");
            mb.Sql("DELETE FROM JournalEntry;");
            mb.Sql("DELETE FROM FiscalPeriod;");
            mb.Sql("DELETE FROM FiscalYear;");
            mb.Sql("DELETE FROM AccountingMapping;");
            mb.Sql("DELETE FROM AccountingSettings;");
            mb.Sql("UPDATE InternalBankAccount SET LedgerAccountId = NULL;");
            mb.Sql("DELETE FROM LedgerAccount;");

            // ===== 2) Catálogo (ordenar por longitud y luego código para que el padre exista) =====
            foreach (var r in Catalog.OrderBy(a => a.Code.Length).ThenBy(a => a.Code))
            {
                var code = r.Code;
                var name = (r.Name ?? "").Replace("'", "''");
                var notes = (r.Notes ?? "").Replace("'", "''");
                var len = code.Length;
                var accountType = code[0] - '0';
                var level = LevelFromLength(len);
                var normalSide = NormalSide(r.Naturaleza, accountType);
                var parentCode = ParentCode(code);
                var parentClause = parentCode == null ? "NULL" : $"(SELECT Id FROM LedgerAccount WHERE Code = N'{parentCode}')";
                var notesClause = string.IsNullOrEmpty(notes) ? "NULL" : $"N'{notes}'";
                var fsClause = string.IsNullOrEmpty(r.Fs) ? "NULL" : $"N'{r.Fs}'";
                var isSystem = (len == 1 || code == "3107003" || code == "3108") ? 1 : 0;

                mb.Sql(
                    "INSERT INTO LedgerAccount " +
                    "(Code, Name, AccountType, Level, NormalBalanceSide, ParentId, " +
                    " IsPostable, IsActive, IsSystem, RequiresCostCenter, RequiresTercero, FinancialStatement, Description, " +
                    " ExpenseAccountId, IncomeAccountId, CreationDate, CreatedBy) " +
                    $"VALUES (N'{code}', N'{name}', {accountType}, {level}, {normalSide}, {parentClause}, " +
                    $" {(r.Postable ? 1 : 0)}, 1, {isSystem}, {(r.RequiresCC ? 1 : 0)}, {(r.RequiresT ? 1 : 0)}, {fsClause}, {notesClause}, " +
                    $" NULL, NULL, '2026-05-01T00:00:00', N'system');"
                );
            }

            // ===== 3) Centros de costo =====
            foreach (var (code, name, desc) in CostCenters)
            {
                var nameE = name.Replace("'", "''");
                var descE = (desc ?? "").Replace("'", "''");
                var descClause = string.IsNullOrEmpty(descE) ? "NULL" : $"N'{descE}'";
                mb.Sql($"INSERT INTO CostCenter (Code, Name, Description, IsActive, CreationDate, CreatedBy) VALUES (N'{code}', N'{nameE}', {descClause}, 1, '2026-05-01T00:00:00', N'system');");
            }

            // ===== 4) AccountingSettings (toggle apagado) =====
            mb.Sql("INSERT INTO AccountingSettings (AutoPostingEnabled) VALUES (0);");

            // ===== 5) AccountingMapping (cuentas de sistema apuntando a los códigos nuevos) =====
            foreach (var (key, code) in Mappings)
            {
                mb.Sql(
                    "INSERT INTO AccountingMapping ([Key], LedgerAccountId) " +
                    $"SELECT {key}, (SELECT Id FROM LedgerAccount WHERE Code = N'{code}') " +
                    $"WHERE NOT EXISTS (SELECT 1 FROM AccountingMapping WHERE [Key] = {key});"
                );
            }
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM AccountingMapping;");
            mb.Sql("DELETE FROM AccountingSettings;");
            mb.Sql("DELETE FROM CostCenter;");
            mb.Sql("UPDATE InternalBankAccount SET LedgerAccountId = NULL;");
            mb.Sql("DELETE FROM JournalEntryLine;");
            mb.Sql("UPDATE FiscalYear SET ClosingJournalEntryId = NULL;");
            mb.Sql("DELETE FROM JournalEntry;");
            mb.Sql("DELETE FROM FiscalPeriod;");
            mb.Sql("DELETE FROM FiscalYear;");
            mb.Sql("DELETE FROM LedgerAccount;");
        }

        private static int LevelFromLength(int len) => len switch
        {
            1 => 1,
            2 => 2,
            4 => 3,
            7 => 4,
            9 => 5,
            _ => throw new InvalidOperationException($"Longitud de código inválida: {len}")
        };

        private static int NormalSide(string? naturaleza, int accountType)
        {
            if (naturaleza == "Deudora") return 1;
            if (naturaleza == "Acreedora") return 2;
            return accountType switch
            {
                1 or 5 or 6 or 7 => 1,
                _ => 2
            };
        }

        private static string? ParentCode(string code) => code.Length switch
        {
            1 => null,
            2 => code.Substring(0, 1),
            4 => code.Substring(0, 2),
            7 => code.Substring(0, 4),
            9 => code.Substring(0, 7),
            _ => null
        };
    }
}
