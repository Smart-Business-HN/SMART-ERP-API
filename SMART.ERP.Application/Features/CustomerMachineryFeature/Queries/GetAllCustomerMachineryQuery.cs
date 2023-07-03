using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerMachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.CustomerMachineryFeature.Queries
{
    public class GetAllCustomerMachineryQuery : IRequest<PagedResponse<List<CustomerMachineryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllCustomerMachineryQueryHandler : IRequestHandler<GetAllCustomerMachineryQuery, PagedResponse<List<CustomerMachineryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<CustomerMachinery> _repositoryAsync;

            public GetAllCustomerMachineryQueryHandler(IMapper mapper,

                IRepositoryAsync<CustomerMachinery> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<CustomerMachineryDto>>> Handle(GetAllCustomerMachineryQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var customersMachinery = await _repositoryAsync.ListAsync(new PaginationCustomerMachinerySpecification(request.PageNumber, request.PageSize));
                var dto = _mapper.Map<List<CustomerMachineryDto>>(customersMachinery);
                return new PagedResponse<List<CustomerMachineryDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
