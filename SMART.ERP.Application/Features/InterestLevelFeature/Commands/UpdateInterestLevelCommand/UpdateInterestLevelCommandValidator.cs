using FluentValidation;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.UpdateInterestLevelCommand
{
    public class UpdateInterestLevelCommandValidator : AbstractValidator<UpdateInterestLevelCommand>
    {
        public UpdateInterestLevelCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
