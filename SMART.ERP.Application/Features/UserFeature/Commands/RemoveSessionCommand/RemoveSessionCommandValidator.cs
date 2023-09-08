using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.RemoveSessionCommand
{
    public class RemoveSessionCommandValidator : AbstractValidator<RemoveSessionCommand>
    {
        public RemoveSessionCommandValidator()
        {
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
