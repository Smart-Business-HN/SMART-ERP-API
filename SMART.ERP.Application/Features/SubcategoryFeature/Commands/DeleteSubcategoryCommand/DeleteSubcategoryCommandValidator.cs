using FluentValidation;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.DeleteSubcategoryCommand
{
    public class DeleteSubcategoryCommandValidator : AbstractValidator<DeleteSubcategoryCommand>
    {
        public DeleteSubcategoryCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
