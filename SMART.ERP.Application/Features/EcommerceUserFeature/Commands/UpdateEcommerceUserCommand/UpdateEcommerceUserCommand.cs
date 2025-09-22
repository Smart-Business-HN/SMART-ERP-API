using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.EcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserCommand;

public class UpdateEcommerceUserCommand : IRequest<Response<EcommerceUserDto>>
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
}
public class UpdateEcommerceUserCommandHandler : IRequestHandler<UpdateEcommerceUserCommand, Response<EcommerceUserDto>>
{
    public readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    public readonly IRepositoryAsync<Department> _departmentRepositoryAsync;
    public readonly IRepositoryAsync<Gender> _genderRepositoryAsync;
    public readonly IMapper _mapper;

    public UpdateEcommerceUserCommandHandler(IRepositoryAsync<EcommerceUser> repositoryAsync, IMapper mapper, IRepositoryAsync<Department> departmentRepositoryAsync, IRepositoryAsync<Gender> genderRepositoryAsync)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
        _departmentRepositoryAsync = departmentRepositoryAsync;
        _genderRepositoryAsync = genderRepositoryAsync;
    }
    public async Task<Response<EcommerceUserDto>> Handle(UpdateEcommerceUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification("",request.Id),cancellationToken);
        var department = null as Department;
        if (user == null) throw new ApiException("Usuario no encontrado");
        if (request.DepartmentId.HasValue)
        {
            department = await _departmentRepositoryAsync.GetByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (department == null && request.DepartmentId.HasValue) throw new ApiException("Departamento no encontrado");
        }
        var gender = await _genderRepositoryAsync.GetByIdAsync(request.GenderId, cancellationToken);
        if(gender == null ) throw new ApiException("Género no encontrado");
        if (user.Email != request.Email)
        {
            var previousUser = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.Email,null), cancellationToken);
            if(previousUser != null && previousUser.Id != user.Id) 
                throw new ApiException("Este correo ya está en uso por otro usuario");
            user.Email = request.Email;
        }

        if (user.UserName != request.UserName)
        {
            var userName = await _repositoryAsync.FirstOrDefaultAsync(new FilterEcommerceUserSpecification(request.UserName,null), cancellationToken);
            if(userName != null && userName.Id != user.Id)
                throw new ApiException("Este nombre de usuario ya está en uso por otro usuario");
            user.UserName = request.UserName;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.DepartmentId = request.DepartmentId;
        user.GenderId = request.GenderId;
        user.BirthDay = request.BirthDay;
        user.ModificationDate = DateTime.UtcNow;
        await _repositoryAsync.UpdateAsync(user, cancellationToken);
        user.Department = department;
        user.Gender = gender;
        var userDto = _mapper.Map<EcommerceUserDto>(user);
        return new Response<EcommerceUserDto>(userDto);
    }
}
