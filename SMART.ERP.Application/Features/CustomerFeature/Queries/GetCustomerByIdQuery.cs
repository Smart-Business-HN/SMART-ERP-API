using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerMachinerySpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetCustomerByIdQuery : IRequest<Response<CustomerDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Response<CustomerDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IRepositoryAsync<CustomerMachinery> _repositoryCustomerMachinery;
        private readonly IMapper _mapper;
        public GetCustomerByIdQueryHandler(IRepositoryAsync<Customer> repositoryHNAsync,
            IRepositoryAsync<Customer> repositoryAsync, IMapper mapper, IRepositoryAsync<CustomerMachinery> repositoryCustomerMachinery)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _repositoryCustomerMachinery = repositoryCustomerMachinery;
        }
        public async Task<Response<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var getLocalCustomer = await _repositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.Id));
            if (getLocalCustomer == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var getRemoteCustomer = await _repositoryHNAsync.FirstOrDefaultAsync(new FilterClientByIdSpecification(getLocalCustomer!.Id));
            if (getRemoteCustomer == null)
            {
                throw new KeyNotFoundException($"Ocurrio un problema al buscar este cliente");
            }
            var machinery = await _repositoryCustomerMachinery.ListAsync(new FilterCustomerMachineryByCustomerIdSpecification(getLocalCustomer!.Id));
            var dtoMachinery = _mapper.Map<List<CustomerMachineryDto>>(machinery);
            var dto = _mapper.Map<CustomerDto>(getRemoteCustomer);
            dto.CustomerMachinery = dtoMachinery;
            return new Response<CustomerDto>(dto);
        }
    }
}
