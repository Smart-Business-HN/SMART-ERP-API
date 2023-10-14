using FluentValidation;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.CreateProductImageCommand
{
    public class CreateProductImageCommandValidator : AbstractValidator<CreateProductImageCommand>
    {
        public CreateProductImageCommandValidator()
        {
            RuleFor(p => p.FileName)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(400).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Url)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");
        }
    }
}
