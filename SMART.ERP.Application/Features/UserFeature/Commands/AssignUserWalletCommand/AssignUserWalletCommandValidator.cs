using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.AssignUserWalletCommand
{
    public class AssignUserWalletCommandValidator : AbstractValidator<AssignUserWalletCommand>
    {
        public AssignUserWalletCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
