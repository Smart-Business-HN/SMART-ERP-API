using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.DeleteCustomerMachineryCommand
{
    public class DeleteCustomerMachineryCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteCustomerMachineryCommandHandler : IRequestHandler<DeleteCustomerMachineryCommand, Response<string>>
    {
        private readonly IRepositoryAsync<CustomerMachinery> _repositoryAsync;

        public DeleteCustomerMachineryCommandHandler(IRepositoryAsync<CustomerMachinery> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteCustomerMachineryCommand request, CancellationToken cancellationToken)
        {
            var customerMachinery = await _repositoryAsync.GetByIdAsync(request.Id);
            if (customerMachinery == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(customerMachinery);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Maquinaria eliminada correctamente", "Eliminado correctamente");
        }
    }
}
