using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.UpdateCustomerMachineryCommand
{
    public class UpdateCustomerMachineryCommand : IRequest<Response<CustomerMachineryDto>>
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int ProductId { get; set; }
        public int BaseInfoId { get; set; }
    }
    public class UpdateCustomerMachineryCommandHandler : IRequestHandler<UpdateCustomerMachineryCommand, Response<CustomerMachineryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<CustomerMachinery> _repositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        public UpdateCustomerMachineryCommandHandler(IMapper mapper, IRepositoryAsync<CustomerMachinery> repositoryAsync, IRepositoryAsync<Product> productRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
        }
        public async Task<Response<CustomerMachineryDto>> Handle(UpdateCustomerMachineryCommand request, CancellationToken cancellationToken)
        {
            var checkIfCustomerMachineryExist = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkIfCustomerMachineryExist == null)
            {
                throw new KeyNotFoundException($"Esta maquinaria no se encuentra registrada");
            }
            var checkIfCustomerExist = await _customerRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (checkIfCustomerExist == null)
            {
                throw new KeyNotFoundException($"Este cliente no se encuentra registrado");
            }
            var checkIfProductExist = await _productRepositoryAsync.GetByIdAsync(request.ProductId);
            if (checkIfProductExist == null)
            {
                throw new KeyNotFoundException($"Este producto no se encuentra registrado");
            }
            checkIfCustomerMachineryExist.BaseInfoId = request.BaseInfoId;
            checkIfCustomerMachineryExist.ProductId = request.ProductId;
            checkIfCustomerMachineryExist.CustomerId = request.CustomerId;
            await _repositoryAsync.UpdateAsync(checkIfCustomerMachineryExist);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CustomerMachineryDto>(checkIfCustomerMachineryExist);
            return new Response<CustomerMachineryDto>(dto, "Actualizado Correctamente");
        }
    }
}
