using FluentValidation;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.CreateSubcategoryCommand
{
    public class CreateSubcategoryCommandValidator : AbstractValidator<CreateSubcategoryCommand>
    {
        public CreateSubcategoryCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
