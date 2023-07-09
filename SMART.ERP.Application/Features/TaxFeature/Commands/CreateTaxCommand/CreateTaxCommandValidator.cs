using FluentValidation;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.CreateTaxCommand
{
    public class CreateTaxCommandValidator : AbstractValidator<CreateTaxCommand>
    {
        public CreateTaxCommandValidator() {
            RuleFor(p => p.Name)
                  .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                  .MaximumLength(10).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.TextForDocuments)
              .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
              .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.Rate)
              .NotNull().WithMessage("{PropertyName} no puede ser null")
              .GreaterThan(-1).WithMessage("\"{PropertyName} no puede ser menor a cero.");
        }
    }
}
