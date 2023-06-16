using FluentValidation;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.CreateLossReasonCommand
{
    public class CreateLossReasonCommandValidator : AbstractValidator<CreateLossReasonCommand>
    {
        public CreateLossReasonCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
