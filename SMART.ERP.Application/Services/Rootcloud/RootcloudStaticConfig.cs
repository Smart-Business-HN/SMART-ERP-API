using SMART.ERP.Application.DTOs.Rootcloud;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public static class RootcloudStaticConfig
    {
        public static string baseUri = "https://machinelink.rootcloud.com";

        public static List<MaintenanceIntervalDto> MaintenanceIntervals()
        {
            var intervals = new List<MaintenanceIntervalDto>()
            {
                new MaintenanceIntervalDto(){
                    Interval = 400,
                    Initial = 100,
                    Categories = new List<int>(){ 27, 15, 16 }
                },
                    new MaintenanceIntervalDto()
                {
                    Interval = 250,
                    Initial = 100,
                    Categories = new List<int>(){23, 24, 12, 13, 26 }
                }
            };

            return intervals;
        }

        public static string UnitOfMeasurementRootcloud(int modelType)
        {
            string result = string.Empty;
            var unitOfMDtos = new List<UnitOfMDto>()
            {
                new UnitOfMDto(){
                    ModelId = 5286,
                    FieldAlias = "rsv_06",
                    TransformationUnit = "100km/l"
                },
                new UnitOfMDto(){
                    ModelId = 10366,
                    FieldAlias = "total_fuel_consumption",
                    TransformationUnit = "Lt"
                },
                new UnitOfMDto(){
                    ModelId = 10338,
                    FieldAlias = "total_fuel_consumption",
                    TransformationUnit = "Lt"
                },
                new UnitOfMDto(){
                    ModelId = 10344,
                    FieldAlias = "total_fuel_consumption",
                    TransformationUnit = "Lt"
                },
                new UnitOfMDto(){
                    ModelId = 10094,
                    FieldAlias = "total_fuel_consumption",
                    TransformationUnit = "L/100KM"
                },
                new UnitOfMDto(){
                    ModelId = 10098,
                    FieldAlias = "total_fuel_consumption",
                    TransformationUnit = "100km/h"
                },
            };
            var unit = unitOfMDtos.FirstOrDefault(a => a.ModelId == modelType);

            if (unit != null)
                result = unit.TransformationUnit;


            return result;
        }

        public static List<DeviceCategoryDto> GetDeviceCategories()
        {
            List<DeviceCategoryDto> categories = new List<DeviceCategoryDto>()
            {
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Volqueta",
                        Selector = "Camión volquete-Camión volquete"
                    }
                },
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Motoniveladora",
                        Selector = "Maquinaria vial-Motoniveladora"
                    }
                },
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Cargadora",
                        Selector = "Excavadora-Cargador"
                    }
                },
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Mixer",
                        Selector = "Maquinaria de hormigón-Camión mezclador"
                    }
                },
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Rodillo",
                        Selector = "Maquinaria vial-Rodillo"
                    }
                },
                {
                    new DeviceCategoryDto()
                    {
                        Name = "Excavadora",
                        Selector = "Excavadora-Excavadora mediana"
                    }
                },
            };

            return categories;
        }

        public static List<string> GetFieldNames()
        {
            List<string> names = new List<string>()
            {
                "Timestamp_Local",
                "country",
                "position",
                "DeviceStatus",
                "GPS_Latitude",
                "GPS_Longitude",
                "Average_fuel_consumption",
                "realtime_fuel_consumption",
                "fuel_level",
                "work_time",
                "engine_hr",
                "iding_time",
                "total_mileage",
                "total_fuel_consumption",
                "rsv_06",
                "machine_no",
                "water_temperature",
                "working_hours",
                "engine_speed",
                "mileage",
                "working_time",
                "standby_time",
                "boot_time",
                "water_temperature",
                "total_mileage",
                "pump2_current",
                "pump1_current",
                "pump02_pressure",
                "pump01_pressure",
                "gear",
                "oil_pressure",
                "cooling_water_temperature",
            };

            return names;
        }


        public static decimal WorkingHour(WorkingItemDto selectFirst)
        {
            var listHr = new List<decimal>();
            switch (selectFirst)
            {
                case var value when selectFirst.Working_hours > 0:
                    listHr.Add(value.Working_hours);
                    break;
                case var value when selectFirst.Working_time > 0 && selectFirst.Engine_hr == 0:
                    listHr.Add(value.Working_time);
                    break;
                case var value when selectFirst.Work_time > 0 && selectFirst.Engine_hr == 0:
                    listHr.Add(value.Work_time);
                    break;
                case var value when selectFirst.Rsv_06 > 0:
                    listHr.Add(value.Rsv_06);
                    break;
                default:
                    listHr.Add(selectFirst.Engine_hr);
                    break;
            }

            return listHr.Count > 0 ? listHr.Max() : 0;
        }
    }
}
