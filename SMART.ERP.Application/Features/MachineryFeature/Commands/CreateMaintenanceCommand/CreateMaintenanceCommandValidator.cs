using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMaintenanceCommand
{
    public class CreateMaintenanceCommandValidator : AbstractValidator<CreateMaintenanceCommand>
    {
        public CreateMaintenanceCommandValidator()
        {
            RuleFor(x => x.Hourmeter)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.MachineryId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.CreationDate)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
