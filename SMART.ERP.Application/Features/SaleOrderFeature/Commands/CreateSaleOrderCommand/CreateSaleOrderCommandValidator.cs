using FluentValidation;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Commands.CreateSaleOrderCommand
{
    public class CreateSaleOrderCommandValidator : AbstractValidator<CreateSaleOrderCommand>
    {
        public CreateSaleOrderCommandValidator()
        {
            RuleFor(p => p.OpportunityId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.SaleOrderProducts.Count)
                .NotEqual(0).WithMessage("No se puede crear un carrito sin al menos un producto");

            RuleForEach(p => p.SaleOrderProducts).ChildRules(dts =>
            {
                dts.RuleFor(p => p.ProductId)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                    .NotEqual(0).WithMessage("{PropertyName} en la caracteristica no puede ser igual a cero");

                dts.RuleFor(p => p.Quantity)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                    .NotEqual(0).WithMessage("{PropertyName} en la caracteristica no puede ser igual a cero");
            });
        }
    }
}
