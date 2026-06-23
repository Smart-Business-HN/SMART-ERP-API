using MediatR;
using SMART.ERP.Application.DTOs.BulkImport;
using SMART.ERP.Application.Services.ExcelImportService;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries.GetProductImportTemplateQuery
{
    /// <summary>
    /// Genera la plantilla .xlsx para la importacion masiva de productos.
    /// </summary>
    public class GetProductImportTemplateQuery : IRequest<byte[]>
    {
    }

    public class GetProductImportTemplateQueryHandler : IRequestHandler<GetProductImportTemplateQuery, byte[]>
    {
        private readonly IExcelImportService _excelImportService;

        public GetProductImportTemplateQueryHandler(IExcelImportService excelImportService)
        {
            _excelImportService = excelImportService;
        }

        public Task<byte[]> Handle(GetProductImportTemplateQuery request, CancellationToken cancellationToken)
        {
            var bytes = _excelImportService.GenerateTemplate("Productos", ProductImportColumns.GetDefinitions());
            return Task.FromResult(bytes);
        }
    }

    /// <summary>
    /// Definicion de columnas de la plantilla de importacion de productos. Las llaves foraneas
    /// (Subcategoria, Marca, Unidad, Estado, Proveedor, Impuesto) se resuelven POR NOMBRE.
    /// </summary>
    public static class ProductImportColumns
    {
        public static ExcelColumnDefinition[] GetDefinitions() =>
        [
            new() { HeaderTitle = "*Codigo",           PropertyName = "Code",            IsRequired = true,  ExampleValue = "PROD-001" },
            new() { HeaderTitle = "*Nombre",           PropertyName = "Name",            IsRequired = true,  ExampleValue = "Taladro Inalambrico 20V" },
            new() { HeaderTitle = "Descripcion",       PropertyName = "Description",     IsRequired = false, ExampleValue = "Taladro 20V con bateria y cargador" },
            new() { HeaderTitle = "*Precio Costo",     PropertyName = "CostPrice",       IsRequired = true,  ExampleValue = "850.00" },
            new() { HeaderTitle = "*Precio Venta",     PropertyName = "SalePrice",       IsRequired = true,  ExampleValue = "1200.00" },
            new() { HeaderTitle = "*Stock",            PropertyName = "CurrentStock",    IsRequired = true,  ExampleValue = "25" },
            new() { HeaderTitle = "*Stock Minimo",     PropertyName = "MinStock",        IsRequired = true,  ExampleValue = "5" },
            new() { HeaderTitle = "*Subcategoria",     PropertyName = "SubcategoryName", IsRequired = true,  ExampleValue = "Herramientas Electricas" },
            new() { HeaderTitle = "*Marca",            PropertyName = "BrandName",       IsRequired = true,  ExampleValue = "DeWalt" },
            new() { HeaderTitle = "*Unidad de Medida", PropertyName = "UnitName",        IsRequired = true,  ExampleValue = "Unidad" },
            new() { HeaderTitle = "*Estado",           PropertyName = "StatusName",      IsRequired = true,  ExampleValue = "Disponible" },
            new() { HeaderTitle = "*Proveedor",        PropertyName = "ProviderName",    IsRequired = true,  ExampleValue = "Ferreteria Central" },
            new() { HeaderTitle = "*Impuesto",         PropertyName = "TaxName",         IsRequired = true,  ExampleValue = "ISV 15%" },
        ];
    }
}
