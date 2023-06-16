using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;

namespace SMART.ERP.Application.Services.AssignUserToOpportunityService
{
    public class AssignUserToOpportunityService : IAssignUserToOpportunityService
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public AssignUserToOpportunityService(IRepositoryAsync<User> repositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }
        public async Task<Guid> FindValidUser()
        {
            var salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", null));
            salesAdvisors = salesAdvisors.FindAll(x => x.FullName != "Oscar Flores");
            int numOpportunity = 0;
            var selectedUser = new User();
            foreach (var user in salesAdvisors)
            {
                var listOpportunity = await _opportunityRepositoryAsync.CountAsync(new FilterOpportunityByUserSpecification(user.Id, true));
                if (listOpportunity == 0)
                {
                    selectedUser = user;
                    break;
                }
                else
                {
                    if (numOpportunity == 0)
                    {
                        numOpportunity = listOpportunity;
                        selectedUser = user;
                    }
                    else
                    {
                        if (listOpportunity < numOpportunity)
                        {
                            numOpportunity = listOpportunity;
                            selectedUser = user;
                        }
                    }
                }
            }
            return selectedUser!.Id;
        }

        public async Task<Guid> FindValidDepartmentUser(int departmentId)
        {
            var salesAdvisors = await _repositoryAsync.ListAsync(new FilterSalesAdvisorByDepartmentSpecification(departmentId));
            if (salesAdvisors.Count == 0)
            {
                return await this.FindValidUser();
            }
            int numOpportunity = 0;
            var selectedUser = new User();
            foreach (var user in salesAdvisors)
            {
                var listOpportunity = await _opportunityRepositoryAsync.CountAsync(new FilterOpportunityByUserSpecification(user.Id, true));
                if (listOpportunity == 0)
                {
                    selectedUser = user;
                    break;
                }
                else
                {
                    if (numOpportunity == 0)
                    {
                        numOpportunity = listOpportunity;
                        selectedUser = user;
                    }
                    else
                    {
                        if (listOpportunity < numOpportunity)
                        {
                            numOpportunity = listOpportunity;
                            selectedUser = user;
                        }
                    }
                }
            }
            return selectedUser!.Id;
        }
    }
}
