using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateFailureReportCommand
{
    public class CreateFailureReportCommandValidator : AbstractValidator<CreateFailureReportCommand>
    {
        public CreateFailureReportCommandValidator()
        {
            RuleFor(p => p.Description)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .MaximumLength(500).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.StatusId)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.MachineryFailureId)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.MachineryId)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.CreationDate)
               .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
