using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.AssignUserToProspectService
{
    public class AssignUserToProspectService : IAssignUserToProspectService
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public AssignUserToProspectService(IRepositoryAsync<Prospect> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public async Task<Guid> AssignUserToProspect()
        {
            var salesAdvisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", null));
            salesAdvisors = salesAdvisors.FindAll(x => x.FullName != "Oscar Flores");
            int numOpportunity = 0;
            var selectedUser = new User();
            foreach (var user in salesAdvisors)
            {
                var listProspect = await _repositoryAsync.CountAsync(new FilterProspectByUserSpecification(user.Id));
                if (listProspect == 0)
                {
                    selectedUser = user;
                    break;
                }
                else
                {
                    if (numOpportunity == 0)
                    {
                        numOpportunity = listProspect;
                        selectedUser = user;
                    }
                    else
                    {
                        if (listProspect < numOpportunity)
                        {
                            numOpportunity = listProspect;
                            selectedUser = user;
                        }
                    }
                }
            }
            return selectedUser.Id;
        }

        public async Task<Guid> FindValidUserDepartment(int departmentId)
        {
            var salesAdvisors = await _userRepositoryAsync.ListAsync(new FilterSalesAdvisorByDepartmentSpecification(departmentId));
            salesAdvisors = salesAdvisors.FindAll(x => x.FullName != "Oscar Flores");
            if (salesAdvisors.Count == 0)
            {
                return await AssignUserToProspect();
            }
            int numOpportunity = 0;
            var selectedUser = new User();
            foreach (var user in salesAdvisors)
            {
                var listProspect = await _repositoryAsync.CountAsync(new FilterProspectByUserSpecification(user.Id));
                if (listProspect == 0)
                {
                    selectedUser = user;
                    break;
                }
                else
                {
                    if (numOpportunity == 0)
                    {
                        numOpportunity = listProspect;
                        selectedUser = user;
                    }
                    else
                    {
                        if (listProspect < numOpportunity)
                        {
                            numOpportunity = listProspect;
                            selectedUser = user;
                        }
                    }
                }
            }
            return selectedUser.Id;
        }
    }
}
