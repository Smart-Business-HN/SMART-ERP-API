using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminResetEcommerceUserPasswordCommand;

public class AdminResetEcommerceUserPasswordCommandValidator : AbstractValidator<AdminResetEcommerceUserPasswordCommand>
{
    public AdminResetEcommerceUserPasswordCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("El Id es requerido.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("La Contraseña es requerida.")
            .Matches(@"^(?=.*[A-Z])(?=.*\d).{8,}$")
            .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
    }
}
