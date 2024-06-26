using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

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
        private readonly IMapper _mapper;
        public GetCustomerByIdQueryHandler(IRepositoryAsync<Customer> repositoryHNAsync,
            IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;

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
            var dto = _mapper.Map<CustomerDto>(getRemoteCustomer);
            return new Response<CustomerDto>(dto);
        }
    }
}
