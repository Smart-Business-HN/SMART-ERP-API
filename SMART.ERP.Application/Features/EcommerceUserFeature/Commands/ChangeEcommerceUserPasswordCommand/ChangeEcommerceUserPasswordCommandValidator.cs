using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.ChangeEcommerceUserPasswordCommand;

public class ChangeEcommerceUserPasswordCommandValidator : AbstractValidator<ChangeEcommerceUserPasswordCommand>
{
    public ChangeEcommerceUserPasswordCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id es requerido.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La contraseña actual es requerida.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
    }
}
