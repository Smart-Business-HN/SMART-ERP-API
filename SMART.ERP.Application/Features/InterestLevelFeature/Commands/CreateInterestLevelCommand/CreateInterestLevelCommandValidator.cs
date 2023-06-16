using FluentValidation;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.CreateInterestLevelCommand
{
    public class CreateInterestLevelCommandValidator : AbstractValidator<CreateInterestLevelCommand>
    {
        public CreateInterestLevelCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
