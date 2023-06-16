namespace SMART.ERP.Application.Services.RegisterClientService
{
    public interface IRegisterClientService
    {
        Task<bool> RegiterClient(Guid clientId, Guid? userId);
    }
}
