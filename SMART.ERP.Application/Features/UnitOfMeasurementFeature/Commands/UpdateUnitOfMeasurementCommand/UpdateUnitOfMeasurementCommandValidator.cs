using FluentValidation;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.UpdateUnitOfMeasurementCommand
{
    public class UpdateUnitOfMeasurementCommandValidator : AbstractValidator<UpdateUnitOfMeasurementCommand>
    {
        public UpdateUnitOfMeasurementCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Abreviation)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(10).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
