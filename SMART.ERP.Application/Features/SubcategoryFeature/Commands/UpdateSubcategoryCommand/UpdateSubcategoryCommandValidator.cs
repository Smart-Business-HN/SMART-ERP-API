using FluentValidation;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.UpdateSubcategoryCommand
{
    public class UpdateSubcategoryCommandValidator : AbstractValidator<Subcategory>
    {
        public UpdateSubcategoryCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
