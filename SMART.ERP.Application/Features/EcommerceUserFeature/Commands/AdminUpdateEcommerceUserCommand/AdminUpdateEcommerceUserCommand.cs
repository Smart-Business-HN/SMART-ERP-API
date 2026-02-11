using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminUpdateEcommerceUserCommand;

public class AdminUpdateEcommerceUserCommand : IRequest<Response<EcommerceUserDto>>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public int GenderId { get; set; }
    public DateTime? BirthDay { get; set; }
    public int CustomerTypeId { get; set; }
    public bool IsActive { get; set; }
}

public class AdminUpdateEcommerceUserCommandHandler : IRequestHandler<AdminUpdateEcommerceUserCommand, Response<EcommerceUserDto>>
{
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
    private readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
    private readonly IRepositoryAsync<CustomerType> _customerTypeRepositoryAsync;
    private readonly IRepositoryAsync<LogEcommerceUser> _logRepositoryAsync;
    private readonly IMapper _mapper;

    public AdminUpdateEcommerceUserCommandHandler(
        IRepositoryAsync<EcommerceUser> repositoryAsync,
        IMapper mapper,
        IRepositoryAsync<Department> departmentRepositoryAsync,
        IRepositoryAsync<Gender> genderRepositoryAsync,
        IRepositoryAsync<CustomerType> customerTypeRepositoryAsync,
        IRepositoryAsync<LogEcommerceUser> logRepositoryAsync)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
        _departmentRepositoryAsync = departmentRepositoryAsync;
        _genderRepositoryAsync = genderRepositoryAsync;
        _customerTypeRepositoryAsync = customerTypeRepositoryAsync;
        _logRepositoryAsync = logRepositoryAsync;
    }

    public async Task<Response<EcommerceUserDto>> Handle(AdminUpdateEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification("", request.Id), cancellationToken);
        if (user == null) throw new ApiException("Usuario no encontrado");

        var department = null as Department;
        if (request.DepartmentId.HasValue)
        {
            department = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (department == null) throw new ApiException("Departamento no encontrado");
        }

        var gender = await _genderRepositoryAsync.GetByIdAsync(request.GenderId, cancellationToken);
        if (gender == null) throw new ApiException("Género no encontrado");

        var customerType = await _customerTypeRepositoryAsync.GetByIdAsync(request.CustomerTypeId, cancellationToken);
        if (customerType == null) throw new ApiException("Tipo de cliente no encontrado");

        var changes = new List<string>();
        if (user.Email != request.Email) changes.Add("Email");
        if (user.UserName != request.UserName) changes.Add("UserName");
        if (user.FirstName != request.FirstName) changes.Add("FirstName");
        if (user.LastName != request.LastName) changes.Add("LastName");
        if (user.PhoneNumber != request.PhoneNumber) changes.Add("PhoneNumber");
        if (user.DepartmentId != request.DepartmentId) changes.Add("DepartmentId");
        if (user.GenderId != request.GenderId) changes.Add("GenderId");
        if (user.BirthDay != request.BirthDay) changes.Add("BirthDay");
        if (user.CustomerTypeId != request.CustomerTypeId) changes.Add("CustomerTypeId");
        if (user.IsActive != request.IsActive) changes.Add("IsActive");

        if (user.Email != request.Email)
        {
            var previousUser = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email, null), cancellationToken);
            if (previousUser != null && previousUser.Id != user.Id)
                throw new ApiException("Este correo ya está en uso por otro usuario");
            user.Email = request.Email;
        }

        if (user.UserName != request.UserName)
        {
            var userName = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.UserName, null), cancellationToken);
            if (userName != null && userName.Id != user.Id)
                throw new ApiException("Este nombre de usuario ya está en uso por otro usuario");
            user.UserName = request.UserName;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FullName = request.FirstName + " " + request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.DepartmentId = request.DepartmentId;
        user.GenderId = request.GenderId;
        user.BirthDay = request.BirthDay;
        user.CustomerTypeId = request.CustomerTypeId;
        user.CustomerType = null;
        user.IsActive = request.IsActive;
        user.ModificationDate = DateTime.UtcNow;
        await _repositoryAsync.UpdateAsync(user, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);
        if (changes.Count > 0)
        {
            await _logRepositoryAsync.AddAsync(new LogEcommerceUser
            {
                EcommerceUserId = user.Id,
                ActionType = (int)EcommerceUserActionType.ProfileUpdate,
                Details = string.Join(", ", changes),
                CreationDate = DateTime.UtcNow
            }, cancellationToken);
            await _logRepositoryAsync.SaveChangesAsync(cancellationToken);
        }

        user.Department = department;
        user.Gender = gender;
        user.CustomerType = customerType;
        var userDto = _mapper.Map<EcommerceUserDto>(user);
        return new Response<EcommerceUserDto>(userDto);
    }
}
