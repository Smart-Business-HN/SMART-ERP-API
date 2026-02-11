using FluentValidation;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminUpdateCartItemCommand;

public class AdminUpdateCartItemCommandValidator : AbstractValidator<AdminUpdateCartItemCommand>
{
    public AdminUpdateCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .GreaterThan(0).WithMessage("El Id del item del carrito debe ser mayor a 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo.")
            .When(x => x.Discount.HasValue);
    }
}
