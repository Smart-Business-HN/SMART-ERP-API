using FluentValidation;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.DeleteInterestLevelCommand
{
    public class DeleteInterestLevelCommandValidator : AbstractValidator<DeleteInterestLevelCommand>
    {
        public DeleteInterestLevelCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
