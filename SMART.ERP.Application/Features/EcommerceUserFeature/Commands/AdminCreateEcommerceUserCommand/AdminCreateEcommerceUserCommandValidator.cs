using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminCreateEcommerceUserCommand;

public class AdminCreateEcommerceUserCommandValidator : AbstractValidator<AdminCreateEcommerceUserCommand>
{
    public AdminCreateEcommerceUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El Email es requerido.")
            .EmailAddress().WithMessage("El Email no es valido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La Contraseña es requerida.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Primer Nombre es requerido.")
            .MaximumLength(50).WithMessage("Primer Nombre no debe exceder 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Apellido es requerido.")
            .MaximumLength(50).WithMessage("Apellido no debe exceder 50 caracteres.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{4}-\d{4}$").WithMessage("El número telefónico debe tener el formato 0000-0000.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.GenderId)
            .NotEqual(0).WithMessage("El Género es requerido.");

        RuleFor(x => x.CustomerTypeId)
            .NotEqual(0).WithMessage("El Tipo de Cliente es requerido.");
    }
}
