
using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerMachineryFeature.Commands.CreateCustomerMachineryCommand
{
    public class CreateCustomerMachineryCommand : IRequest<Response<CustomerMachineryDto>>
    {
        public Guid CustomerId { get; set; }
        public int ProductId { get; set; }
        public int BaseInfoId { get; set; }

    }
    public class CreateCustomerMachineryCommandHandler : IRequestHandler<CreateCustomerMachineryCommand, Response<CustomerMachineryDto>>
    {
        private readonly IRepositoryAsync<CustomerMachinery> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Product> _productRepositoryAsync;
        public CreateCustomerMachineryCommandHandler(IRepositoryAsync<CustomerMachinery> repositoryAsync, IMapper mapper, IRepositoryAsync<Customer> clientRepositoryAsync, IRepositoryAsync<Product> productRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _clientRepositoryAsync = clientRepositoryAsync;
            _productRepositoryAsync = productRepositoryAsync;
        }

        public async Task<Response<CustomerMachineryDto>> Handle(CreateCustomerMachineryCommand request, CancellationToken cancellationToken)
        {
            var customer = await _clientRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
            if (customer == null)
            {
                throw new ApiException($"No existe un cliente con el id {request.CustomerId}");
            }
            var product = await _productRepositoryAsync.FirstOrDefaultAsync(new FilterProductSpecification(null, request.ProductId));
            if (product == null)
            {
                throw new ApiException($"No existe un producto con el id {request.ProductId}");
            }
            var newRecord = _mapper.Map<CustomerMachinery>(request);
            newRecord.CustomerId = customer.Id;
            newRecord.ProductId = request.ProductId;
            newRecord.BaseInfoId = request.BaseInfoId;
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<CustomerMachineryDto>(response);
            return new Response<CustomerMachineryDto>(dto);
        }
    }
}
