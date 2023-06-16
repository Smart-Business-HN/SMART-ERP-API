using FluentValidation;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.UpdateBaseProductCommand
{
    public class UpdateBaseProductCommandValidator : AbstractValidator<UpdateBaseProductCommand>
    {
        public UpdateBaseProductCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => x.Description != null);

            RuleFor(p => p.BrandId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");

            RuleFor(p => p.UnitOfMeasurementId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");

            RuleFor(p => p.SubCategoryId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");

            RuleFor(p => p.StatusId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");

            RuleFor(p => p.ProviderId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
