namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class WorkingItemDto
    {
        public string Time { get; set; }
        public string BaseIndoId { get; set; }
        public string SerialNum { get; set; }
        public string DeviceName { get; set; }
        public string Timestamp_Local { get; set; }
        public string Country { get; set; }
        public string Position { get; set; }
        public string DeviceStatus { get; set; }
        public dynamic GPS_Latitude { get; set; }
        public dynamic GPS_Longitude { get; set; }
        public dynamic Average_fuel_consumption { get; set; }
        public dynamic Realtime_fuel_consumption { get; set; }
        public dynamic Fuel_level { get; set; }
        public dynamic Work_time { get; set; }
        public dynamic Engine_hr { get; set; }
        public dynamic Iding_time { get; set; }
        public dynamic Total_mileage { get; set; }
        public dynamic Total_fuel_consumption { get; set; }
        public dynamic Rsv_06 { get; set; }
        public string Machine_no { get; set; }
        public dynamic Water_temperature { get; set; }
        public dynamic Working_hours { get; set; }
        public dynamic Engine_speed { get; set; }
        public dynamic Mileage { get; set; }
        public dynamic Working_time { get; set; }
        public dynamic Standby_time { get; set; }
        public dynamic Boot_time { get; set; }
        public dynamic Pump2_current { get; set; }
        public dynamic Pump1_current { get; set; }
        public dynamic Pump02_pressure { get; set; }
        public dynamic Pump01_pressure { get; set; }
        public dynamic Gear { get; set; }
        public dynamic Oil_pressure { get; set; }
        public dynamic Cooling_water_temperature { get; set; }

        public WorkingItemDto(string time, string baseIndoId, string serialNum, string deviceName, string timestamp_Local,
            string country, string position, string deviceStatus, dynamic gPS_Latitude, dynamic gPS_Longitude, dynamic average_fuel_consumption,
            dynamic realtime_fuel_consumption, dynamic fuel_level, dynamic work_time, dynamic engine_hr, dynamic iding_time,
            dynamic total_mileage, dynamic total_fuel_consumption, dynamic rsv_06, string machine_no, dynamic water_temperature,
            dynamic working_hours, dynamic engine_speed, dynamic mileage, dynamic working_time, dynamic standby_time, dynamic boot_time,
            dynamic pump2_current, dynamic pump1_current, dynamic pump02_pressure, dynamic pump01_pressure, dynamic gear, dynamic oil_pressure,
            dynamic cooling_water_temperature)
        {
            Time = time;
            BaseIndoId = baseIndoId;
            SerialNum = serialNum;
            DeviceName = deviceName;
            Timestamp_Local = timestamp_Local;
            Country = country;
            Position = position;
            DeviceStatus = deviceStatus;
            GPS_Latitude = Convert.ToString(gPS_Latitude) == "-" ? 0 : Convert.ToSingle(gPS_Latitude);
            GPS_Longitude = Convert.ToString(gPS_Longitude) == "-" ? 0 : Convert.ToSingle(gPS_Longitude);
            Average_fuel_consumption = Convert.ToString(average_fuel_consumption) == "-" ? 0 : Convert.ToDecimal(average_fuel_consumption);
            Realtime_fuel_consumption = Convert.ToString(realtime_fuel_consumption) == "-" ? 0 : Convert.ToDecimal(realtime_fuel_consumption);
            Fuel_level = Convert.ToString(fuel_level) == "-" ? 0 : Convert.ToDecimal(fuel_level);
            Work_time = Convert.ToString(work_time) == "-" ? 0 : Convert.ToDecimal(work_time);
            Engine_hr = Convert.ToString(engine_hr) == "-" ? 0 : Convert.ToDecimal(engine_hr);
            Iding_time = Convert.ToString(iding_time) == "-" ? 0 : Convert.ToDecimal(iding_time);
            Total_mileage = Convert.ToString(total_mileage) == "-" ? 0 : Convert.ToDecimal(total_mileage);
            Total_fuel_consumption = Convert.ToString(total_fuel_consumption) == "-" ? 0 : Convert.ToDecimal(total_fuel_consumption);
            Rsv_06 = Convert.ToString(rsv_06) == "-" ? 0 : Convert.ToDecimal(rsv_06);
            Machine_no = machine_no;
            Water_temperature = Convert.ToString(water_temperature) == "-" ? 0 : Convert.ToDecimal(water_temperature);
            Working_hours = Convert.ToString(working_hours) == "-" ? 0 : Convert.ToDecimal(working_hours);
            Engine_speed = Convert.ToString(engine_speed) == "-" ? 0 : Convert.ToDecimal(engine_speed);
            Mileage = Convert.ToString(mileage) == "-" ? 0 : Convert.ToDecimal(mileage);
            Working_time = Convert.ToString(working_time) == "-" ? 0 : Convert.ToDecimal(working_time);
            Standby_time = Convert.ToString(standby_time) == "-" ? 0 : Convert.ToDecimal(standby_time);
            Boot_time = Convert.ToString(boot_time) == "-" ? 0 : Convert.ToDecimal(boot_time);
            Pump2_current = Convert.ToString(pump2_current) == "-" ? 0 : Convert.ToDecimal(pump2_current);
            Pump1_current = Convert.ToString(pump1_current) == "-" ? 0 : Convert.ToDecimal(pump1_current);
            Pump02_pressure = Convert.ToString(pump02_pressure) == "-" ? 0 : Convert.ToDecimal(pump02_pressure);
            Pump01_pressure = Convert.ToString(pump01_pressure) == "-" ? 0 : Convert.ToDecimal(pump01_pressure);
            Gear = Convert.ToString(gear) == "-" ? 0 : Convert.ToDecimal(gear);
            Oil_pressure = Convert.ToString(oil_pressure) == "-" ? 0 : Convert.ToDecimal(oil_pressure);
            Cooling_water_temperature = Convert.ToString(cooling_water_temperature) == "-" ? 0 : Convert.ToDecimal(cooling_water_temperature);
        }
    }
}
