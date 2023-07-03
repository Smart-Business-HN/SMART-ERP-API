using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.ClientSpecification;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Customer;

namespace SMART.ERP.Application.Features.UserFeature.Queries
{
    public class GetUserWalletByIdQuery : IRequest<Response<UserWalletDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserWalletByIdQueryHandler : IRequestHandler<GetUserWalletByIdQuery, Response<UserWalletDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryHNAsync;
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<AdvisorGoal> _advisorGoalRepositoryAsync;

        public GetUserWalletByIdQueryHandler(IRepositoryAsync<Customer> repositoryHNAsync, IRepositoryAsync<Customer> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IMapper mapper, IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync)
        {
            _repositoryHNAsync = repositoryHNAsync;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _mapper = mapper;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _advisorGoalRepositoryAsync = advisorGoalRepositoryAsync;
        }

        public async Task<Response<UserWalletDto>> Handle(GetUserWalletByIdQuery request, CancellationToken cancellationToken)
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
            var activeUserOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterOpportunityByUserSpecification(request.Id, true));
            var activeOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(null, true));
            var assignedClients = await _repositoryAsync.ListAsync(new FilterCustomerByUserSpecification(request.Id));
            foreach (var customer in assignedClients)
            {
                var client = clients.FirstOrDefault(x => x.Id == customer.Id);
                var assignedClientActiveOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveOpportunitiesByCustomerSpecification(customer.Id, true));
                assignedClientActiveOpportunities = assignedClientActiveOpportunities.FindAll(x => x.UserId == request.Id);
                var clientWallet = _mapper.Map<ClientWalletDto>(client);
                clientWallet.Assigned = true;
                clientWallet.NumOpportunities = assignedClientActiveOpportunities.Count;
                clientWallet.NumProducts = 0;
                clientWallet.UserId = customer.UserId;
                foreach (var opp in assignedClientActiveOpportunities)
                {
                    if (opp.QuoteProducts != null)
                    {
                        foreach (var quote in opp.QuoteProducts)
                        {
                            clientWallet.NumProducts += quote.Quantity;
                        }
                    }
                    clientWallet.Opportunities = _mapper.Map<List<OpportunityWalletDto>>(assignedClientActiveOpportunities);
                }
                userWallet.Clients.Add(clientWallet);
            }
            foreach (var opp in activeUserOpportunities)
            {
                if (userWallet.Clients.Exists(x => x.Id == opp.Customer!.Id))
                {
                    continue;
                }
                var client = clients.FirstOrDefault(x => x.Id == opp.Customer!.Id);
                if (client == null)
                {
                    throw new KeyNotFoundException($"No se encontro el cliente con id {opp.Customer!.Id}");
                }
                var clientWallet = _mapper.Map<ClientWalletDto>(client);
                var customerOpportunities = activeOpportunities.FindAll(x => x.CustomerId == opp.CustomerId && x.UserId == request.Id);
                clientWallet.NumOpportunities = customerOpportunities.Count;
                clientWallet.NumProducts = 0;
                clientWallet.UserId = opp.Customer!.UserId;
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

            //Advisor Monthly Goal Calcs
            var currentDate = DateTime.Now;
            var monthGoal = await _advisorGoalRepositoryAsync.FirstOrDefaultAsync(new FilterAdvisorGoalByMonthSpecification(request.Id, null, currentDate));
            var annualGoal = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(currentDate.Year, request.Id));
            var response = new AdvisorDashboardDto();
            if (monthGoal == null)
            {
                response.MonthlyGoal = 0;
                response.MonthlyGoalPercentage = 0;
            }
            else
            {
                var monthWonOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInMonthYearSpecification(currentDate.Month, currentDate.Year, request.Id, null));
                var total = 0m;
                foreach (var opportunity in monthWonOpportunities)
                {
                    total += opportunity.Total;
                }
                response.MonthlyGoal = monthGoal.Goal;
                decimal totalCompletion = 0;
                if (monthGoal.Goal > 0)
                {
                    totalCompletion = total / monthGoal.Goal * 100;
                }
                else
                {
                    totalCompletion = 0;
                }

                if (totalCompletion > 100)
                {
                    response.MonthlyGoalPercentage = 100;
                }
                else
                {
                    response.MonthlyGoalPercentage = Math.Round(totalCompletion, 2, MidpointRounding.AwayFromZero);
                }
            }
            if (annualGoal.Count < 0)
            {
                response.AnnualGoal = 0;
                response.AnnualGoalPercentage = 0;
            }
            else
            {
                foreach (var goal in annualGoal)
                {
                    response.AnnualGoal += goal.Goal;
                }

                decimal totalCompletion = 0;
                var yearSales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(currentDate.Year, request.Id));
                yearSales = yearSales.FindAll(x => x.OpportunityStep!.Name == "Ganado");
                foreach (var opportunity in yearSales)
                {
                    totalCompletion += opportunity.Total;
                }

                if (response.AnnualGoal > 0)
                {
                    response.AnnualGoalPercentage = Math.Round(totalCompletion / response.AnnualGoal * 100, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    response.AnnualGoalPercentage = 0;
                }

                if (response.AnnualGoalPercentage > 100)
                {
                    response.AnnualGoalPercentage = 100;
                }
            }
            userWallet.AdvisorInfo = response;
            return new Response<UserWalletDto>(userWallet);

        }
    }
}
