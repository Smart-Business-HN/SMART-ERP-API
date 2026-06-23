using System.Globalization;
using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.BulkImport;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.BaseProductFeature.Queries.GetProductImportTemplateQuery;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.ExcelImportService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Specifications.BrandSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Specifications.SubcategorySpecification;
using SMART.ERP.Application.Specifications.TaxSpecification;
using SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.BulkImportProductsCommand
{
    /// <summary>
    /// Importacion masiva de productos desde una plantilla .xlsx. Todo-o-nada: si hay cualquier error
    /// de fila no se inserta ningun producto y se devuelve la lista completa de errores. Las llaves
    /// foraneas se resuelven por nombre. Replica los efectos del alta individual (slug, ProductSubcategory,
    /// PriceListItem por defecto y eviccion de cache).
    /// </summary>
    public class BulkImportProductsCommand : IRequest<Response<BulkImportResultDto>>
    {
        public IFormFile File { get; set; } = null!;
    }

    public class BulkImportProductsCommandHandler : IRequestHandler<BulkImportProductsCommand, Response<BulkImportResultDto>>
    {
        private const int MaxRows = 500;
        private const long MaxFileSize = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = [".xlsx"];

        private readonly IRepositoryAsync<Product> _productRepository;
        private readonly IRepositoryAsync<Subcategory> _subcategoryRepository;
        private readonly IRepositoryAsync<Brand> _brandRepository;
        private readonly IRepositoryAsync<Tax> _taxRepository;
        private readonly IRepositoryAsync<Status> _statusRepository;
        private readonly IRepositoryAsync<Provider> _providerRepository;
        private readonly IRepositoryAsync<UnitOfMeasurement> _unitRepository;
        private readonly IRepositoryAsync<ProductSubcategory> _productSubcategoryRepository;
        private readonly IRepositoryAsync<PriceListItem> _priceListItemRepository;
        private readonly IExcelImportService _excelImportService;
        private readonly IJwtService _jwtService;
        private readonly IPriceListService _priceListService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public BulkImportProductsCommandHandler(
            IRepositoryAsync<Product> productRepository,
            IRepositoryAsync<Subcategory> subcategoryRepository,
            IRepositoryAsync<Brand> brandRepository,
            IRepositoryAsync<Tax> taxRepository,
            IRepositoryAsync<Status> statusRepository,
            IRepositoryAsync<Provider> providerRepository,
            IRepositoryAsync<UnitOfMeasurement> unitRepository,
            IRepositoryAsync<ProductSubcategory> productSubcategoryRepository,
            IRepositoryAsync<PriceListItem> priceListItemRepository,
            IExcelImportService excelImportService,
            IJwtService jwtService,
            IPriceListService priceListService,
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            _productRepository = productRepository;
            _subcategoryRepository = subcategoryRepository;
            _brandRepository = brandRepository;
            _taxRepository = taxRepository;
            _statusRepository = statusRepository;
            _providerRepository = providerRepository;
            _unitRepository = unitRepository;
            _productSubcategoryRepository = productSubcategoryRepository;
            _priceListItemRepository = priceListItemRepository;
            _excelImportService = excelImportService;
            _jwtService = jwtService;
            _priceListService = priceListService;
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<Response<BulkImportResultDto>> Handle(BulkImportProductsCommand request, CancellationToken cancellationToken)
        {
            // 1. Validacion del archivo
            ValidateFile(request.File);

            // 2. Parsear filas
            var columns = ProductImportColumns.GetDefinitions();
            var rows = _excelImportService.ParseRows(request.File, "Productos", columns);

            if (rows.Count == 0)
                throw new ApiException("El archivo no contiene datos para importar.");

            if (rows.Count > MaxRows)
                throw new ApiException($"El archivo excede el limite de {MaxRows} filas. El archivo contiene {rows.Count} filas.");

            // 3. Validacion por campo
            var errors = new List<BulkImportRowError>();
            ValidateRows(rows, errors);

            // 4. Duplicados dentro del archivo
            DetectIntraFileDuplicates(rows, errors);

            // 5. Resolver llaves foraneas por nombre
            var subcategoryMap = await ResolveSubcategoriesAsync(rows, errors, cancellationToken);
            var brandMap = await ResolveBrandsAsync(rows, errors, cancellationToken);
            var unitMap = await ResolveUnitsAsync(rows, errors, cancellationToken);
            var statusMap = await ResolveStatusesAsync(rows, errors, cancellationToken);
            var providerMap = await ResolveProvidersAsync(rows, errors, cancellationToken);
            var taxMap = await ResolveTaxesAsync(rows, errors, cancellationToken);

            // 6. Duplicados contra la BD (nombre y codigo)
            await CheckProductNameDuplicatesAsync(rows, errors, cancellationToken);
            await CheckProductCodeDuplicatesAsync(rows, errors, cancellationToken);

            // 7. Todo-o-nada: si hay errores, no se inserta nada
            if (errors.Count > 0)
            {
                return new Response<BulkImportResultDto>(
                    $"Se encontraron {errors.Count} error(es) en el archivo. No se importo ningun producto.")
                {
                    Data = new BulkImportResultDto
                    {
                        TotalRows = rows.Count,
                        SuccessCount = 0,
                        ErrorCount = errors.Count,
                        Errors = errors.OrderBy(e => e.Row).ThenBy(e => e.Field).ToList()
                    }
                };
            }

            // 8. Construir entidades (todas las filas son validas)
            var createdBy = _jwtService.GetSubjectToken();
            var now = DateTime.Now;
            var products = new List<Product>();
            var productTaxRates = new List<(Product Product, decimal TaxRate)>();

            foreach (var row in rows)
            {
                var name = row.Values["Name"]!;
                var taxName = row.Values["TaxName"]!;
                var tax = taxMap[taxName.ToLowerInvariant()];

                var product = new Product
                {
                    Code = row.Values["Code"]!,
                    Name = name,
                    Slug = Regex.Replace(Regex.Replace(name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-"),
                    Description = row.Values.GetValueOrDefault("Description"),
                    ProductType = ProductType.Tangible,
                    IsFatherProduct = false,
                    CostPrice = decimal.Parse(row.Values["CostPrice"]!, CultureInfo.InvariantCulture),
                    RecomendedSalePrice = decimal.Parse(row.Values["SalePrice"]!, CultureInfo.InvariantCulture),
                    CurrentStock = int.Parse(row.Values["CurrentStock"]!, CultureInfo.InvariantCulture),
                    MinStock = int.Parse(row.Values["MinStock"]!, CultureInfo.InvariantCulture),
                    SubCategoryId = subcategoryMap[row.Values["SubcategoryName"]!.ToLowerInvariant()],
                    BrandId = brandMap[row.Values["BrandName"]!.ToLowerInvariant()],
                    UnitOfMeasurementId = unitMap[row.Values["UnitName"]!.ToLowerInvariant()],
                    StatusId = statusMap[row.Values["StatusName"]!.ToLowerInvariant()],
                    ProviderId = providerMap[row.Values["ProviderName"]!.ToLowerInvariant()],
                    TaxId = tax.Id,
                    IsActive = true,
                    ShowInEcommerce = false,
                    CreationDate = now,
                    CreatedBy = createdBy
                };

                products.Add(product);
                productTaxRates.Add((product, tax.Rate));
            }

            // 9. Insertar atomicamente: productos -> ProductSubcategory -> PriceListItem por defecto
            var defaultPriceListId = await _priceListService.GetDefaultPriceListIdAsync(cancellationToken);
            var priceItemsAdded = false;

            await _unitOfWork.ExecuteInTransactionAsync(async (ct) =>
            {
                // AddRangeAsync (Ardalis) hace SaveChanges internamente -> asigna los Id generados.
                await _productRepository.AddRangeAsync(products, ct);

                // Membresia principal de subcategoria (replica el alta individual)
                var subcategoryRows = products
                    .Select(p => new ProductSubcategory { ProductId = p.Id, SubcategoryId = p.SubCategoryId })
                    .ToList();
                await _productSubcategoryRepository.AddRangeAsync(subcategoryRows, ct);

                // Precio por defecto: ceil(costo * 1.30 * (1 + tasaImpuesto/100)) en la lista default
                if (defaultPriceListId.HasValue)
                {
                    var priceItems = new List<PriceListItem>();
                    foreach (var (product, taxRate) in productTaxRates)
                    {
                        if (product.CostPrice <= 0) continue;
                        var price = Math.Ceiling((product.CostPrice * 1.30m) * (1m + (taxRate / 100m)));
                        priceItems.Add(new PriceListItem
                        {
                            PriceListId = defaultPriceListId.Value,
                            ProductId = product.Id,
                            Price = price,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = createdBy
                        });
                    }

                    if (priceItems.Count > 0)
                    {
                        await _priceListItemRepository.AddRangeAsync(priceItems, ct);
                        priceItemsAdded = true;
                    }
                }
            }, cancellationToken);

            // 10. Eviccion de cache (una vez, tras confirmar)
            if (priceItemsAdded)
                await _outputCacheStore.EvictByTagAsync("cache_pricelists", cancellationToken);
            await ProductCacheEviction.EvictAsync(_outputCacheStore, cancellationToken);

            return new Response<BulkImportResultDto>(
                new BulkImportResultDto
                {
                    TotalRows = rows.Count,
                    SuccessCount = products.Count,
                    ErrorCount = 0,
                    Errors = []
                },
                $"Se importaron {products.Count} producto(s) exitosamente.");
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ApiException("Debe seleccionar un archivo.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ApiException("Solo se permiten archivos Excel (.xlsx).");

            if (file.Length > MaxFileSize)
                throw new ApiException("El archivo excede el tamano maximo permitido de 5MB.");
        }

        private static void ValidateRows(List<ExcelImportRow> rows, List<BulkImportRowError> errors)
        {
            foreach (var row in rows)
            {
                var code = row.Values.GetValueOrDefault("Code");
                var name = row.Values.GetValueOrDefault("Name");
                var description = row.Values.GetValueOrDefault("Description");
                var costPrice = row.Values.GetValueOrDefault("CostPrice");
                var salePrice = row.Values.GetValueOrDefault("SalePrice");
                var stock = row.Values.GetValueOrDefault("CurrentStock");
                var minStock = row.Values.GetValueOrDefault("MinStock");

                // Codigo - requerido, max 20
                if (string.IsNullOrWhiteSpace(code))
                    Add(errors, row, "*Codigo", "El codigo es requerido.");
                else if (code.Length > 20)
                    Add(errors, row, "*Codigo", "El codigo no puede exceder 20 caracteres.");

                // Nombre - requerido, max 50
                if (string.IsNullOrWhiteSpace(name))
                    Add(errors, row, "*Nombre", "El nombre es requerido.");
                else if (name.Length > 50)
                    Add(errors, row, "*Nombre", "El nombre no puede exceder 50 caracteres.");

                // Descripcion - opcional, max 600
                if (description != null && description.Length > 600)
                    Add(errors, row, "Descripcion", "La descripcion no puede exceder 600 caracteres.");

                // Precio Costo - requerido, numerico >= 0
                if (string.IsNullOrWhiteSpace(costPrice))
                    Add(errors, row, "*Precio Costo", "El precio de costo es requerido.");
                else if (!decimal.TryParse(costPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out var cp) || cp < 0)
                    Add(errors, row, "*Precio Costo", "El precio de costo debe ser un numero mayor o igual a 0.");

                // Precio Venta - requerido, numerico >= 0
                if (string.IsNullOrWhiteSpace(salePrice))
                    Add(errors, row, "*Precio Venta", "El precio de venta es requerido.");
                else if (!decimal.TryParse(salePrice, NumberStyles.Number, CultureInfo.InvariantCulture, out var sp) || sp < 0)
                    Add(errors, row, "*Precio Venta", "El precio de venta debe ser un numero mayor o igual a 0.");

                // Stock - requerido, entero >= 0
                if (string.IsNullOrWhiteSpace(stock))
                    Add(errors, row, "*Stock", "El stock es requerido.");
                else if (!int.TryParse(stock, NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) || s < 0)
                    Add(errors, row, "*Stock", "El stock debe ser un numero entero mayor o igual a 0.");

                // Stock Minimo - requerido, entero >= 0
                if (string.IsNullOrWhiteSpace(minStock))
                    Add(errors, row, "*Stock Minimo", "El stock minimo es requerido.");
                else if (!int.TryParse(minStock, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms) || ms < 0)
                    Add(errors, row, "*Stock Minimo", "El stock minimo debe ser un numero entero mayor o igual a 0.");

                // Llaves foraneas - solo se valida presencia aqui; la existencia se resuelve despues
                RequireFk(errors, row, "SubcategoryName", "*Subcategoria", "La subcategoria es requerida.");
                RequireFk(errors, row, "BrandName", "*Marca", "La marca es requerida.");
                RequireFk(errors, row, "UnitName", "*Unidad de Medida", "La unidad de medida es requerida.");
                RequireFk(errors, row, "StatusName", "*Estado", "El estado es requerido.");
                RequireFk(errors, row, "ProviderName", "*Proveedor", "El proveedor es requerido.");
                RequireFk(errors, row, "TaxName", "*Impuesto", "El impuesto es requerido.");
            }
        }

        private static void RequireFk(List<BulkImportRowError> errors, ExcelImportRow row, string property, string field, string message)
        {
            if (string.IsNullOrWhiteSpace(row.Values.GetValueOrDefault(property)))
                Add(errors, row, field, message);
        }

        private static void DetectIntraFileDuplicates(List<ExcelImportRow> rows, List<BulkImportRowError> errors)
        {
            // Codigo duplicado dentro del archivo
            DetectDuplicateGroup(rows, errors, "Code", "*Codigo",
                n => $"Codigo duplicado en el archivo (mismo valor en fila {n}).");

            // Nombre duplicado dentro del archivo
            DetectDuplicateGroup(rows, errors, "Name", "*Nombre",
                n => $"Nombre duplicado en el archivo (mismo valor en fila {n}).");
        }

        private static void DetectDuplicateGroup(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, string property, string field, Func<int, string> message)
        {
            var groups = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Values.GetValueOrDefault(property)))
                .GroupBy(r => r.Values[property]!.ToLowerInvariant())
                .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                var firstRow = group.First().RowNumber;
                foreach (var row in group.Skip(1))
                {
                    Add(errors, row, field, message(firstRow));
                }
            }
        }

        private async Task<Dictionary<string, int>> ResolveSubcategoriesAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "SubcategoryName");
            var map = new Dictionary<string, int>();
            if (names.Count == 0) return map;

            var existing = await _subcategoryRepository.ListAsync(new SubcategoriesByNamesSpecification(names), ct);
            var byName = existing.GroupBy(s => s.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var row in rows)
            {
                var name = row.Values.GetValueOrDefault("SubcategoryName");
                if (string.IsNullOrWhiteSpace(name)) continue;
                var key = name.ToLowerInvariant();

                if (!byName.TryGetValue(key, out var matches))
                {
                    Add(errors, row, "*Subcategoria", $"No existe la subcategoria \"{name}\". Verifique que este registrada en el sistema.");
                }
                else if (matches.Count > 1)
                {
                    Add(errors, row, "*Subcategoria", $"La subcategoria \"{name}\" existe en mas de una categoria. Renombre las subcategorias para que sean unicas o contacte al administrador.");
                }
                else
                {
                    map[key] = matches[0].Id;
                }
            }
            return map;
        }

        private async Task<Dictionary<string, int>> ResolveBrandsAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "BrandName");
            if (names.Count == 0) return new();
            var existing = await _brandRepository.ListAsync(new BrandsByNamesSpecification(names), ct);
            var map = existing.GroupBy(b => b.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.First().Id);
            ReportMissing(rows, errors, "BrandName", "*Marca", map, n => $"No existe la marca \"{n}\". Verifique que este registrada en el sistema.");
            return map;
        }

        private async Task<Dictionary<string, int>> ResolveUnitsAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "UnitName");
            if (names.Count == 0) return new();
            var existing = await _unitRepository.ListAsync(new UnitsOfMeasurementByNamesSpecification(names), ct);
            var map = existing.GroupBy(u => u.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.First().Id);
            ReportMissing(rows, errors, "UnitName", "*Unidad de Medida", map, n => $"No existe la unidad de medida \"{n}\". Verifique que este registrada en el sistema.");
            return map;
        }

        private async Task<Dictionary<string, int>> ResolveStatusesAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "StatusName");
            if (names.Count == 0) return new();
            var existing = await _statusRepository.ListAsync(new StatusesByNamesSpecification(names), ct);
            var map = existing.GroupBy(s => s.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.First().Id);
            ReportMissing(rows, errors, "StatusName", "*Estado", map, n => $"No existe el estado \"{n}\". Verifique que este registrado en el sistema.");
            return map;
        }

        private async Task<Dictionary<string, int>> ResolveProvidersAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "ProviderName");
            if (names.Count == 0) return new();
            var existing = await _providerRepository.ListAsync(new ProvidersByNamesSpecification(names), ct);
            var map = existing.GroupBy(p => p.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.First().Id);
            ReportMissing(rows, errors, "ProviderName", "*Proveedor", map, n => $"No existe el proveedor \"{n}\". Verifique que este registrado en el sistema.");
            return map;
        }

        private async Task<Dictionary<string, Tax>> ResolveTaxesAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "TaxName");
            if (names.Count == 0) return new();
            var existing = await _taxRepository.ListAsync(new TaxesByNamesSpecification(names), ct);
            var map = existing.GroupBy(t => t.Name.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.First());

            foreach (var row in rows)
            {
                var name = row.Values.GetValueOrDefault("TaxName");
                if (!string.IsNullOrWhiteSpace(name) && !map.ContainsKey(name.ToLowerInvariant()))
                    Add(errors, row, "*Impuesto", $"No existe el impuesto \"{name}\". Verifique que este registrado en el sistema.");
            }
            return map;
        }

        private async Task CheckProductNameDuplicatesAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var names = DistinctValues(rows, "Name");
            if (names.Count == 0) return;
            var existing = await _productRepository.ListAsync(new ProductsByNamesSpecification(names), ct);
            var existingNames = existing.Select(p => p.Name.ToLowerInvariant()).ToHashSet();

            foreach (var row in rows)
            {
                var name = row.Values.GetValueOrDefault("Name");
                if (!string.IsNullOrWhiteSpace(name) && existingNames.Contains(name.ToLowerInvariant()))
                    Add(errors, row, "*Nombre", "Ya existe un producto registrado con este nombre.");
            }
        }

        private async Task CheckProductCodeDuplicatesAsync(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, CancellationToken ct)
        {
            var codes = DistinctValues(rows, "Code");
            if (codes.Count == 0) return;
            var existing = await _productRepository.ListAsync(new ProductsByCodesSpecification(codes), ct);
            var existingCodes = existing.Select(p => p.Code.ToLowerInvariant()).ToHashSet();

            foreach (var row in rows)
            {
                var code = row.Values.GetValueOrDefault("Code");
                if (!string.IsNullOrWhiteSpace(code) && existingCodes.Contains(code.ToLowerInvariant()))
                    Add(errors, row, "*Codigo", "Ya existe un producto registrado con este codigo.");
            }
        }

        private static void ReportMissing(
            List<ExcelImportRow> rows, List<BulkImportRowError> errors, string property, string field,
            Dictionary<string, int> map, Func<string, string> message)
        {
            foreach (var row in rows)
            {
                var name = row.Values.GetValueOrDefault(property);
                if (!string.IsNullOrWhiteSpace(name) && !map.ContainsKey(name.ToLowerInvariant()))
                    Add(errors, row, field, message(name));
            }
        }

        private static List<string> DistinctValues(List<ExcelImportRow> rows, string property)
        {
            return rows
                .Select(r => r.Values.GetValueOrDefault(property))
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void Add(List<BulkImportRowError> errors, ExcelImportRow row, string field, string message)
        {
            errors.Add(new BulkImportRowError { Row = row.RowNumber, Field = field, Message = message });
        }
    }
}
