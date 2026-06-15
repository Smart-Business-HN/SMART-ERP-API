using FluentValidation;
using SMART.ERP.Domain.Enums;

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

            RuleFor(p => p.ProductType)
                .IsInEnum().WithMessage("Tipo de producto inválido.");

            RuleFor(p => p.BrandId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UnitOfMeasurementId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.SubCategoryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            When(x => x.SubCategoryIds != null && x.SubCategoryIds.Count > 0, () =>
            {
                RuleForEach(x => x.SubCategoryIds!)
                    .GreaterThan(0).WithMessage("El id de subcategoría adicional debe ser mayor a cero.");
                RuleFor(x => x.SubCategoryIds!)
                    .Must(ids => ids.Distinct().Count() == ids.Count)
                    .WithMessage("No se permiten subcategorías adicionales duplicadas.");
            });

            RuleFor(p => p.StatusId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.ProviderId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            When(x => x.ProductType == ProductType.Combo, () =>
            {
                RuleFor(x => x.Components)
                    .NotNull().WithMessage("Un combo requiere al menos un componente.")
                    .Must(c => c != null && c.Count > 0).WithMessage("Un combo requiere al menos un componente.");
                RuleForEach(x => x.Components!).ChildRules(c =>
                {
                    c.RuleFor(p => p.ProductId).GreaterThan(0).WithMessage("ProductId del componente es requerido.");
                    c.RuleFor(p => p.Quantity).GreaterThan(0).WithMessage("La cantidad del componente debe ser mayor a cero.");
                });
            });

            When(x => x.ProductType != ProductType.Combo, () =>
            {
                RuleFor(x => x.Components)
                    .Must(c => c == null || c.Count == 0)
                    .WithMessage("Sólo los combos pueden tener componentes.");
            });
        }
    }
}
