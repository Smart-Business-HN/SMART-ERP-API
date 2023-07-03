using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class CustomerWalletReportQuery : IRequest<PagedResponse<List<UserWalletDto>>>
    {
        public Guid? Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class CustomerWalletReportQueryHandler : IRequestHandler<CustomerWalletReportQuery, PagedResponse<List<UserWalletDto>>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public CustomerWalletReportQueryHandler(IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IMapper mapper, IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<PagedResponse<List<UserWalletDto>>> Handle(CustomerWalletReportQuery request, CancellationToken cancellationToken)
        {
            var response = new List<UserWalletDto>();
            if (request.Id != null)
            {
                var checkifExist = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(request.Id, null));
                if (checkifExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro el usuario con id {request.Id}");
                }
                var userWallet = new UserWalletDto();
                userWallet.User = _mapper.Map<UserDto>(checkifExist);
                userWallet.Clients = new List<ClientWalletDto>();
                userWallet.NumTemporalClients = 0;
                userWallet.NumOpportunity = 0;
                userWallet.NumClient = 0;
                var clients = await _repositoryHNAsync.ListAsync(new FilterClientByIdSpecification(null));
                var activeUserOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByUserSpecification((Guid)request.Id, false));
                var allOpportunities = await _opportunityRepositoryAsync.ListAsync(new OpportunityIncludesSpecification(null, null));
                var assignedClients = await _repositoryAsync.ListAsync(new FilterCustomerByUserSpecification((Guid)request.Id));
                foreach (var customer in assignedClients)
                {
                    var client = clients.FirstOrDefault(x => x.Id == customer.Id);
                    var assignedClientOpportunities = allOpportunities.FindAll(x => x.CustomerId == customer.Id);
                    assignedClientOpportunities = assignedClientOpportunities.FindAll(x => x.UserId == request.Id);
                    var clientWallet = _mapper.Map<ClientWalletDto>(client);
                    clientWallet.Assigned = true;
                    clientWallet.NumOpportunities = assignedClientOpportunities.Count;
                    clientWallet.NumProducts = 0;
                    clientWallet.UserId = customer.UserId;
                    foreach (var opp in assignedClientOpportunities)
                    {
                        if (opp.QuoteProducts != null)
                        {
                            foreach (var quote in opp.QuoteProducts)
                            {
                                clientWallet.NumProducts += quote.Quantity;
                            }
                        }
                        clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(assignedClientOpportunities);
                    }
                    userWallet.Clients.Add(clientWallet);
                }
                foreach (var opp in activeUserOpportunities)
                {
                    if (userWallet.Clients.Exists(x => x.Id == opp.Customer.Id))
                    {
                        continue;
                    }
                    var client = clients.FirstOrDefault(x => x.Id == opp.Customer.Id);
                    if (client == null)
                    {
                        throw new KeyNotFoundException($"No se encontro el cliente con id {opp.Customer.Id}");
                    }
                    var clientWallet = _mapper.Map<ClientWalletDto>(client);
                    var customerOpportunities = allOpportunities.FindAll(x => x.CustomerId == opp.CustomerId && x.UserId == request.Id);
                    clientWallet.NumOpportunities = customerOpportunities.Count;
                    clientWallet.NumProducts = 0;
                    clientWallet.UserId = opp.Customer.UserId;
                    userWallet.NumTemporalClients += 1;
                    foreach (var opportunity in customerOpportunities)
                    {
                        if (opportunity.QuoteProducts != null)
                        {
                            foreach (var quote in opportunity.QuoteProducts)
                            {
                                clientWallet.NumProducts += quote.Quantity;
                            }
                        }
                    }
                    clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(customerOpportunities);
                    userWallet.Clients.Add(clientWallet);
                }
                userWallet.NumOpportunity = activeUserOpportunities.Count;
                userWallet.NumClient = userWallet.Clients.Count() - userWallet.NumTemporalClients;
                response.Add(userWallet);
            }
            else
            {
                var salesAdvisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", null));
                foreach (var user in salesAdvisors)
                {
                    var userWallet = new UserWalletDto();
                    userWallet.User = _mapper.Map<UserDto>(user);
                    userWallet.Clients = new List<ClientWalletDto>();
                    userWallet.NumTemporalClients = 0;
                    userWallet.NumOpportunity = 0;
                    userWallet.NumClient = 0;
                    var clients = await _repositoryHNAsync.ListAsync(new FilterClientByIdSpecification(null));
                    var activeUserOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByUserSpecification(user.Id, false));
                    var allOpportunities = await _opportunityRepositoryAsync.ListAsync(new OpportunityIncludesSpecification(null, null));
                    var assignedClients = await _repositoryAsync.ListAsync(new FilterCustomerByUserSpecification(user.Id));
                    foreach (var customer in assignedClients)
                    {
                        var client = clients.FirstOrDefault(x => x.Id == customer.Id);
                        var assignedClientOpportunities = allOpportunities.FindAll(x => x.CustomerId == customer.Id);
                        assignedClientOpportunities = assignedClientOpportunities.FindAll(x => x.UserId == user.Id);
                        var clientWallet = _mapper.Map<ClientWalletDto>(client);
                        clientWallet.Assigned = true;
                        clientWallet.NumOpportunities = assignedClientOpportunities.Count;
                        clientWallet.NumProducts = 0;
                        clientWallet.UserId = customer.UserId;
                        foreach (var opp in assignedClientOpportunities)
                        {
                            if (opp.QuoteProducts != null)
                            {
                                foreach (var quote in opp.QuoteProducts)
                                {
                                    clientWallet.NumProducts += quote.Quantity;
                                }
                            }
                            clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(assignedClientOpportunities);
                        }
                        userWallet.Clients.Add(clientWallet);
                    }
                    foreach (var opp in activeUserOpportunities)
                    {
                        if (userWallet.Clients.Exists(x => x.Id == opp.Customer.Id))
                        {
                            continue;
                        }
                        var client = clients.FirstOrDefault(x => x.Id == opp.Customer.Id);
                        if (client == null)
                        {
                            throw new KeyNotFoundException($"No se encontro el cliente con id {opp.Customer.Id}");
                        }
                        var clientWallet = _mapper.Map<ClientWalletDto>(client);
                        var customerOpportunities = allOpportunities.FindAll(x => x.CustomerId == opp.CustomerId && x.UserId == user.Id);
                        clientWallet.NumOpportunities = customerOpportunities.Count;
                        clientWallet.NumProducts = 0;
                        clientWallet.UserId = opp.Customer.UserId;
                        userWallet.NumTemporalClients += 1;
                        foreach (var opportunity in customerOpportunities)
                        {
                            if (opportunity.QuoteProducts != null)
                            {
                                foreach (var quote in opportunity.QuoteProducts)
                                {
                                    clientWallet.NumProducts += quote.Quantity;
                                }
                            }
                        }
                        clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(customerOpportunities);
                        userWallet.Clients.Add(clientWallet);
                    }
                    userWallet.NumOpportunity = activeUserOpportunities.Count;
                    userWallet.NumClient = userWallet.Clients.Count() - userWallet.NumTemporalClients;
                    response.Add(userWallet);
                }

            }
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = response.Count;
            }
            var pagedResult = response.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<UserWalletDto>>(pagedResult, request.PageNumber, request.PageSize, response.Count);
        }
    }
}
