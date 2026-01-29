using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetAllHNClientQuery : IRequest<PagedResponse<List<CustomerDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
    }

    public class GetAllHNClientQueryHandler : IRequestHandler<GetAllHNClientQuery, PagedResponse<List<CustomerDto>>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllHNClientQueryHandler(IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<CustomerDto>>> Handle(GetAllHNClientQuery request, CancellationToken cancellationToken)
        {
            var pageSize = 10;

            var response = new List<CustomerDto>();

            if (!string.IsNullOrEmpty(request.Parameter))
            {
                var listWithFilters = await _repositoryHNAsync.ListAsync(new PaginationClientSpecification(request.Parameter, pageSize, null, null));
                foreach (var client in listWithFilters)
                {
                    var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(client.Id));
                    if (checkIfExist == null)
                    {
                        var dto = _mapper.Map<CustomerDto>(client);
                        response.Add(dto);
                    }
                }
            }
            else
            {
                var customers = await _repositoryHNAsync.ListAsync(new PaginationClientSpecification(null, null, null, null));
                foreach (var item in customers)
                {
                    if (response.Count == pageSize)
                    {
                        break;
                    }
                    var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(item.Id));
                    if (checkIfExist == null)
                    {
                        var dto = _mapper.Map<CustomerDto>(item);
                        response.Add(dto);
                    }
                }
            }
            return new PagedResponse<List<CustomerDto>>(response, request.PageNumber, pageSize, response.Count);
        }
    }
}
