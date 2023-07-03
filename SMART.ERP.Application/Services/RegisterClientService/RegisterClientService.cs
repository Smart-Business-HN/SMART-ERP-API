using SMART.ERP.Application.Repository;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.RegisterClientService
{
    public class RegisterClientService : IRegisterClientService
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;

        public RegisterClientService(IRepositoryAsync<Customer> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<bool> RegiterClient(Guid clientId, Guid? userId)
        {
            try
            {
                var client = new Customer()
                {
                    Id = clientId,
                    RegistrationDate = DateTime.Now,
                    UserId = userId != null ? userId : null
                };
                await _repositoryAsync.AddAsync(client);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
