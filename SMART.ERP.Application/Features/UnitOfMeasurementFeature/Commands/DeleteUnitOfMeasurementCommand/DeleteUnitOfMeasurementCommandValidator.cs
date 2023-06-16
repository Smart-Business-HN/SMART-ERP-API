using FluentValidation;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.DeleteUnitOfMeasurementCommand
{
    public class DeleteUnitOfMeasurementCommandValidator : AbstractValidator<DeleteUnitOfMeasurementCommand>
    {
        public DeleteUnitOfMeasurementCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
