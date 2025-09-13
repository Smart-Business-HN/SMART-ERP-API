using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;

public class CreateEcommerUserValidator : AbstractValidator<CreateEcommerceUserCommand>
{
    public CreateEcommerUserValidator()
    {
        RuleFor(x=>x.Email).NotEmpty().WithMessage("El Email es requerido.");
        RuleFor(x=>x.Email).EmailAddress().WithMessage("El Email no es valido.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La Contraseña es requerida.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
        RuleFor(x=>x.FirstName).NotEmpty().WithMessage("Primer Nombre es requerido.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El número telefónico es requerido.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("El número telefónico debe tener el formato 0000-0000.")
            .When(x => x.PhoneNumber != null);
    }
}