using FluentValidation;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.UpdateProductFtrCommand
{
    public class UpdateProductFtrCommandValidator : AbstractValidator<UpdateProductFtrCommand>
    {
        public UpdateProductFtrCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
