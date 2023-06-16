using FluentValidation;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.UpdateProductImageCommand
{
    public class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
    {
        public UpdateProductImageCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.FileName)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Url)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");
        }
    }
}
