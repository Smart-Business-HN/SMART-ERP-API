namespace SMART.ERP.Application.Services.JwtService
{
    public interface IJwtService
    {
        public string GetSubjectToken();
        public Guid GetUidToken();
    }
}
