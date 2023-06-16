namespace SMART.ERP.Application.Services.AssignUserToOpportunityService
{
    public interface IAssignUserToOpportunityService
    {
        public Task<Guid> FindValidUser();
        public Task<Guid> FindValidDepartmentUser(int departmentId);
    }
}
