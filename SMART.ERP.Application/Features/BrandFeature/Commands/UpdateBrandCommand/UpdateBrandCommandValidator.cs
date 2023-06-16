using FluentValidation;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.UpdateBrandCommand
{
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Description)
                .MaximumLength(200).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => x.Description != null);
        }
    }
}
