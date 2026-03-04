using FluentValidation;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartStatusCommand;

public class AdminUpdateCartStatusCommandValidator : AbstractValidator<AdminUpdateCartStatusCommand>
{
    public AdminUpdateCartStatusCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("{PropertyName} es requerido");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Estado no válido");
    }
}
