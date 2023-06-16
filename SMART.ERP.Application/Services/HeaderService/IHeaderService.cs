namespace SMART.ERP.Application.Services.HeaderService
{
    public interface IHeaderService
    {
        public string GetClientDeviceInfo();
        public string GetClientIP();
        public string GetClientLat();
        public bool VerificatedSecretKey();
        public string GetClientLng();
    }
}
