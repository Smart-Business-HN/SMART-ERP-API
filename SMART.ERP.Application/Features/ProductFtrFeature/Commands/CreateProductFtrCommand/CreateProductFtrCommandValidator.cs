using FluentValidation;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.CreateProductFtrCommand
{
    public class CreateProductFtrCommandValidator : AbstractValidator<CreateProductFtrCommand>
    {
        public CreateProductFtrCommandValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
