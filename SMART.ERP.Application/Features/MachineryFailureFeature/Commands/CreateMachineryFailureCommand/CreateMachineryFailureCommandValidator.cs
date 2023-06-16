using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.CreateMachineryFailureCommand
{
    public class CreateMachineryFailureCommandValidator : AbstractValidator<CreateMachineryFailureCommand>
    {
        public CreateMachineryFailureCommandValidator()
        {
            RuleFor(p => p.Name)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
