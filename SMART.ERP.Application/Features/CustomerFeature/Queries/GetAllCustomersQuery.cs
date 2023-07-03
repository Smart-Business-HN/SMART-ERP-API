using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Domain.Entities;

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

                var response = new List<CustomerDto>();
                try
                {

                var customers = await _repositoryAsync.ListAsync(new CustomerIncludesSpecification());
               
               
                    if (customers != null)
                    {

                        if (!string.IsNullOrEmpty(request.Parameter)
                                || !string.IsNullOrEmpty(request.Order) && !string.IsNullOrEmpty(request.Column))
                        {
                            var listWithFilters = await _repositoryAsync.ListAsync(new PaginationClientSpecification(request.Parameter, null, request.Order, request.Column));
                            foreach (var client in listWithFilters)
                            {
                                var findById = customers.FirstOrDefault(x => x.Id == client.Id);
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
                                guids.Add(item.Id);
                            }

                            var clients = await _repositoryAsync.ListAsync(new FilterClientFromMotors(guids));
                            response = _mapper.Map<List<CustomerDto>>(clients);
                        }
                    }
                    response = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new PagedResponse<List<CustomerDto>>(response, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
                
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
