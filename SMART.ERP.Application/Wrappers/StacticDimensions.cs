namespace SMART.ERP.Application.Wrappers
{
    public class StacticDimensions
    {
        public List<string> GetNames()
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
    }
}
