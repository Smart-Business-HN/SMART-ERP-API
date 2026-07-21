using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.GoogleLoginEcommerceUserCommand;

public class GoogleLoginEcommerceUserCommandValidator : AbstractValidator<GoogleLoginEcommerceUserCommand>
{
    public GoogleLoginEcommerceUserCommandValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty().WithMessage("El token de Google es requerido.");

        // Solo aplica cuando viene el paso 2 del alta; mismo formato que el registro con contraseña.
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{4}-\d{4}$").WithMessage("El número telefónico debe tener el formato 0000-0000.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.GenderId)
            .GreaterThan(0).WithMessage("El género no es válido.")
            .When(x => x.GenderId.HasValue);
    }
}
