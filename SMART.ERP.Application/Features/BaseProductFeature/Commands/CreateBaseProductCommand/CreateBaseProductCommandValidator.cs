using FluentValidation;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.CreateBaseProductCommand
{
    public class CreateBaseProductCommandValidator : AbstractValidator<CreateBaseProductCommand>
    {
        public CreateBaseProductCommandValidator()
        {
            RuleFor(p => p.Code)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(20).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => x.Description != null);

            RuleFor(p => p.BrandId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UnitOfMeasurementId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.SubCategoryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.StatusId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.ProviderId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
