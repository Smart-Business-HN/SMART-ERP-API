using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminUpdateEcommerceUserCommand;

public class AdminUpdateEcommerceUserCommandValidator : AbstractValidator<AdminUpdateEcommerceUserCommand>
{
    public AdminUpdateEcommerceUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id es requerido.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El Email es requerido.")
            .EmailAddress().WithMessage("El Email no es valido.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("El nombre de usuario es requerido.")
            .MaximumLength(50).WithMessage("El nombre de usuario no debe exceder 50 caracteres.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Primer Nombre es requerido.")
            .MaximumLength(50).WithMessage("Primer Nombre no debe exceder 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Apellido es requerido.")
            .MaximumLength(50).WithMessage("Apellido no debe exceder 50 caracteres.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El número telefónico es requerido.");

        RuleFor(x => x.GenderId)
            .NotEqual(0).WithMessage("El Género es requerido.");

        RuleFor(x => x.CustomerTypeId)
            .NotEqual(0).WithMessage("El Tipo de Cliente es requerido.");
    }
}
