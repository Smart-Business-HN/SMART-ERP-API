using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.RefreshTokenCommand
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacío");
        }
    }
}
