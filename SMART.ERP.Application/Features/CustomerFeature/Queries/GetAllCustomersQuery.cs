using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Quotation;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetAllCustomersQuery : IRequest<PagedResponse<List<CustomerDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PagedResponse<List<CustomerDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Customer> _repositoryAsync;

            public GetAllCustomersQueryHandler(IMapper mapper,
                IRepositoryAsync<Customer> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var customers = await _repositoryAsync.ListAsync(new CustomerIncludesSpecification(request.Parameter,request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<CustomerDto>>(customers);
                return new PagedResponse<List<CustomerDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());

            }
        }
    }
}
