using FluentValidation;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Description)
                .MaximumLength(200).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => x.Description != null);
        }
    }
}
