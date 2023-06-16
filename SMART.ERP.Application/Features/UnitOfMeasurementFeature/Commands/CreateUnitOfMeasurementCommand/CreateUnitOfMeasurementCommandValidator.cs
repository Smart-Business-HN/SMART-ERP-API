using FluentValidation;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.CreateUnitOfMeasurementCommand
{
    public class CreateUnitOfMeasurementCommandValidator : AbstractValidator<CreateUnitOfMeasurementCommand>
    {
        public CreateUnitOfMeasurementCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Abreviation)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(10).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
