using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.UpdateMachineryFailureCommand
{
    public class UpdateMachineryFailureCommandValidator : AbstractValidator<UpdateMachineryFailureCommand>
    {
        public UpdateMachineryFailureCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
