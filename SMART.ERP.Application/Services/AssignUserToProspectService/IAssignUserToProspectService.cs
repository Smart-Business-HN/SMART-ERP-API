namespace SMART.ERP.Application.Services.AssignUserToProspectService
{
    public interface IAssignUserToProspectService
    {
        public Task<Guid> AssignUserToProspect();
        public Task<Guid> FindValidUserDepartment(int departmentId);
    }
}
