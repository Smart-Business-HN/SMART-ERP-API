using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetAllBasicInfoCustomerQuery : IRequest<Response<List<BasicInfoCustomerDto>>>
    {
        public class GetAllBasicInfoCustomerQueryHandler : IRequestHandler<GetAllBasicInfoCustomerQuery, Response<List<BasicInfoCustomerDto>>>
        {
            private readonly IRepositoryAsync<Customer> _repositoryAsync;
            private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
            private readonly IMapper _mapper;

            public GetAllBasicInfoCustomerQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IRepositoryHNAsync<Client> repositoryHNAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _repositoryHNAsync = repositoryHNAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<BasicInfoCustomerDto>>> Handle(GetAllBasicInfoCustomerQuery request, CancellationToken cancellationToken)
            {
                var response = new List<BasicInfoCustomerDto>();
                var customers = await _repositoryAsync.ListAsync(new CustomerIncludesSpecification());
                var clients = await _repositoryHNAsync.ListAsync();
                if (customers.Count > 0)
                {
                    foreach (var item in customers)
                    {
                        var getRemoteCustomer = clients.Find(x => x.Id == item.MasterId);

                        if (getRemoteCustomer != null)
                        {
                            var dto = _mapper.Map<BasicInfoCustomerDto>(getRemoteCustomer);
                            dto.MotorsId = item.Id;
                            dto.MasterId = getRemoteCustomer!.Id;
                            dto.AssignedUserId = item.UserId;
                            response.Add(dto);
                        }
                    }
                }
                return new Response<List<BasicInfoCustomerDto>>(response);
            }
        }
    }
}
