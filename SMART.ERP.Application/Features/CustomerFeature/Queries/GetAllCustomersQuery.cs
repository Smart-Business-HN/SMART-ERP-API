using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

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
            private readonly IRepositoryHNAsync<Client> _repositoryHNAsync;
            private readonly IRepositoryAsync<Customer> _repositoryAsync;

            public GetAllCustomersQueryHandler(IMapper mapper,
                IRepositoryHNAsync<Client> repositoryHNAsync,
                IRepositoryAsync<Customer> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryHNAsync = repositoryHNAsync;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var response = new List<CustomerDto>();
                var customers = await _repositoryAsync.ListAsync(new CustomerIncludesSpecification());
                if (customers.Count > 0)
                {
                    if (!string.IsNullOrEmpty(request.Parameter)
                            || !string.IsNullOrEmpty(request.Order) && !string.IsNullOrEmpty(request.Column))
                    {
                        var listWithFilters = await _repositoryHNAsync.ListAsync(new PaginationClientSpecification(request.Parameter, null, request.Order, request.Column));
                        foreach (var client in listWithFilters)
                        {
                            var findById = customers.FirstOrDefault(x => x.MasterId == client.Id);
                            if (findById != null)
                            {
                                var dto = _mapper.Map<CustomerDto>(client);
                                response.Add(dto);
                            }
                        }
                    }
                    else
                    {
                        List<Guid> guids = new();
                        foreach (var item in customers)
                        {
                            guids.Add(item.MasterId);
                        }

                        var clients = await _repositoryHNAsync.ListAsync(new FilterClientFromMotors(guids));
                        response = _mapper.Map<List<CustomerDto>>(clients);
                    }
                }
                response = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                return new PagedResponse<List<CustomerDto>>(response, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
