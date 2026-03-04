using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Infrastructure.Seeders
{
    public static class DropshippingSeed
    {
        public static async Task SeedAsync(DatabaseContext context)
        {
            // 1. Crear WarehouseTypes si no existen
            if (!await context.WarehouseTypes.AnyAsync())
            {
                var types = new List<WarehouseType>
                {
                    new() { Name = "Físico", Description = "Almacén físico propio de Smart Business", IsVirtual = false, IsActive = true },
                    new() { Name = "Virtual", Description = "Almacén virtual para dropshipping", IsVirtual = true, IsActive = true }
                };
                await context.WarehouseTypes.AddRangeAsync(types);
                await context.SaveChangesAsync();
            }

            // 2. Obtener el tipo "Físico"
            var physicalTypeId = await context.WarehouseTypes
                .Where(x => x.Name == "Físico")
                .Select(x => x.Id)
                .FirstAsync();

            // 3. Actualizar warehouses existentes a tipo "Físico" si aún no tienen tipo
            var warehousesWithoutType = await context.Warehouses
                .Where(x => x.WarehouseTypeId == 0)
                .ToListAsync();

            foreach (var warehouse in warehousesWithoutType)
            {
                warehouse.WarehouseTypeId = physicalTypeId;
                warehouse.IsVirtual = false;
            }
            await context.SaveChangesAsync();

            // 4. Obtener el tipo "Virtual"
            var virtualTypeId = await context.WarehouseTypes
                .Where(x => x.Name == "Virtual")
                .Select(x => x.Id)
                .FirstAsync();

            // 5. Buscar proveedores (pueden o no existir)
            var giganet = await context.Providers.FirstOrDefaultAsync(x => x.Name.Contains("Giganet"));
            var diselsa = await context.Providers.FirstOrDefaultAsync(x => x.Name.Contains("Diselsa"));
            var solutionBox = await context.Providers.FirstOrDefaultAsync(x => x.Name.Contains("Solution Box"));

            // 6. Buscar ciudades
            var sanPedroCity = await context.Cities.FirstOrDefaultAsync(x => x.Name == "San Pedro Sula");
            var tegucigalpaCity = await context.Cities.FirstOrDefaultAsync(x => x.Name == "Tegucigalpa");

            // 7. Crear warehouses virtuales si no existen
            var virtualWarehouses = new List<(string name, int? cityId, int? providerId)>
            {
                ("Virtual Giganet SPS", sanPedroCity?.Id, giganet?.Id),
                ("Virtual Diselsa SPS", sanPedroCity?.Id, diselsa?.Id),
                ("Virtual Solution Box TGU", tegucigalpaCity?.Id, solutionBox?.Id)
            };

            foreach (var (name, cityId, providerId) in virtualWarehouses)
            {
                var exists = await context.Warehouses.AnyAsync(w => w.Name == name);
                if (!exists)
                {
                    var warehouse = new Warehouse
                    {
                        Name = name,
                        Address = name.Contains("SPS") ? "San Pedro Sula" : "Tegucigalpa",
                        WarehouseTypeId = virtualTypeId,
                        CityId = cityId,
                        IsVirtual = true,
                        IsGeneralWarehouse = false,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    };
                    await context.Warehouses.AddAsync(warehouse);
                    await context.SaveChangesAsync();

                    // 8. Vincular con proveedor si existe
                    if (providerId.HasValue)
                    {
                        var providerWarehouse = new ProviderWarehouse
                        {
                            ProviderId = providerId.Value,
                            WarehouseId = warehouse.Id,
                            IsActive = true,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = "SYSTEM"
                        };
                        await context.ProviderWarehouses.AddAsync(providerWarehouse);
                    }
                }
            }
            await context.SaveChangesAsync();

            // 9. Actualizar proveedores con información de dropshipping si existen
            if (giganet != null)
            {
                giganet.SupportsDropshipping = true;
                giganet.DefaultShippingCost = 50;
                giganet.DefaultShippingType = "Pickup";
                giganet.DefaultShippingDays = 1;
            }

            if (diselsa != null)
            {
                diselsa.SupportsDropshipping = true;
                diselsa.DefaultShippingCost = 50;
                diselsa.DefaultShippingType = "Pickup";
                diselsa.DefaultShippingDays = 1;
            }

            if (solutionBox != null)
            {
                solutionBox.SupportsDropshipping = true;
                solutionBox.DefaultShippingCost = 160; // Promedio de 120-200
                solutionBox.DefaultShippingType = "Delivery";
                solutionBox.DefaultShippingDays = 3;
            }
            await context.SaveChangesAsync();

            // 10. Crear configuraciones de shipping si los proveedores existen
            var shippingConfigs = new List<ShippingCostConfiguration>();

            if (giganet != null && sanPedroCity != null)
            {
                var exists = await context.ShippingCostConfigurations
                    .AnyAsync(c => c.SourceProviderId == giganet.Id);
                if (!exists)
                {
                    shippingConfigs.Add(new ShippingCostConfiguration
                    {
                        SourceProviderId = giganet.Id,
                        SourceCityId = sanPedroCity.Id,
                        MinCost = 50,
                        MaxCost = 50,
                        DefaultCost = 50,
                        CostType = "Pickup",
                        IsActive = true,
                        Priority = 1,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            if (diselsa != null && sanPedroCity != null)
            {
                var exists = await context.ShippingCostConfigurations
                    .AnyAsync(c => c.SourceProviderId == diselsa.Id);
                if (!exists)
                {
                    shippingConfigs.Add(new ShippingCostConfiguration
                    {
                        SourceProviderId = diselsa.Id,
                        SourceCityId = sanPedroCity.Id,
                        MinCost = 50,
                        MaxCost = 50,
                        DefaultCost = 50,
                        CostType = "Pickup",
                        IsActive = true,
                        Priority = 1,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            if (solutionBox != null && tegucigalpaCity != null)
            {
                var exists = await context.ShippingCostConfigurations
                    .AnyAsync(c => c.SourceProviderId == solutionBox.Id);
                if (!exists)
                {
                    shippingConfigs.Add(new ShippingCostConfiguration
                    {
                        SourceProviderId = solutionBox.Id,
                        SourceCityId = tegucigalpaCity.Id,
                        MinCost = 120,
                        MaxCost = 200,
                        DefaultCost = 160,
                        CostType = "Delivery",
                        IsActive = true,
                        Priority = 1,
                        CreationDate = DateTime.UtcNow,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            if (shippingConfigs.Any())
            {
                await context.ShippingCostConfigurations.AddRangeAsync(shippingConfigs);
                await context.SaveChangesAsync();
            }
        }
    }
}
