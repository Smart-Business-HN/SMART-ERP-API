using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetAllBasicInfoCustomerQuery : IRequest<Response<List<BasicInfoCustomerDto>>>
    {
        public class GetAllBasicInfoCustomerQueryHandler : IRequestHandler<GetAllBasicInfoCustomerQuery, Response<List<BasicInfoCustomerDto>>>
        {
            private readonly IRepositoryAsync<Customer> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllBasicInfoCustomerQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<BasicInfoCustomerDto>>> Handle(GetAllBasicInfoCustomerQuery request, CancellationToken cancellationToken)
            {
                var response = new List<BasicInfoCustomerDto>();
                var customers = await _repositoryAsync.ListAsync(new CustomerIncludesSpecification());
                if (customers.Count > 0)
                {
                    foreach (var item in customers)
                    {
                            var dto = _mapper.Map<BasicInfoCustomerDto>(item);
                            dto.MotorsId = item.Id;
                            dto.AssignedUserId = item.UserId;
                            response.Add(dto);
                    }
                }
                return new Response<List<BasicInfoCustomerDto>>(response);
            }
        }
    }
}
