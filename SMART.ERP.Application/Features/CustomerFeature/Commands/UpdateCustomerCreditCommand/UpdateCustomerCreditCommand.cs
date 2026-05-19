using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCreditCommand
{
    public class UpdateCustomerCreditCommand : IRequest<Response<CustomerDto>>
    {
        public Guid Id { get; set; }
        public bool CreditEnabled { get; set; }
        public decimal CreditLimit { get; set; }
    }

    public class UpdateCustomerCreditCommandHandler : IRequestHandler<UpdateCustomerCreditCommand, Response<CustomerDto>>
    {
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateCustomerCreditCommandHandler(
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IMapper mapper,
            IOutputCacheStore outputCacheStored)
        {
            _customerRepositoryAsync = customerRepositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<CustomerDto>> Handle(UpdateCustomerCreditCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepositoryAsync.FirstOrDefaultAsync(
                new FilterCustomerByMasterIdSpecification(request.Id), cancellationToken);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Este cliente no se encuentra registrado");
            }

            customer.CreditEnabled = request.CreditEnabled;
            customer.CreditLimit = request.CreditLimit;

            await _customerRepositoryAsync.UpdateAsync(customer, cancellationToken);
            await _customerRepositoryAsync.SaveChangesAsync(cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_customers", cancellationToken);

            var dto = _mapper.Map<CustomerDto>(customer);
            return new Response<CustomerDto>(dto, "Crédito del cliente actualizado correctamente");
        }
    }
}
