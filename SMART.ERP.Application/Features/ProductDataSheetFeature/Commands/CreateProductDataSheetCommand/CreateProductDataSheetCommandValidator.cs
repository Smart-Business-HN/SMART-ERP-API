using FluentValidation;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.CreateProductDataSheetCommand
{
    public class CreateProductDataSheetCommandValidator : AbstractValidator<CreateProductDataSheetCommand>
    {
        public CreateProductDataSheetCommandValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.DataSheetId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
