using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.DeleteMachineryFailureCommand
{
    public class DeleteMachineryFailureCommandValidator : AbstractValidator<DeleteMachineryFailureCommand>
    {
        public DeleteMachineryFailureCommandValidator()
        {
            RuleFor(p => p.Id)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
