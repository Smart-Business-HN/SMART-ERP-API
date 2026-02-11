using FluentValidation;

namespace SMART.ERP.Application.Features.CartFeature.Commands.AdminAddProductToCartCommand;

public class AdminAddProductToCartCommandValidator : AbstractValidator<AdminAddProductToCartCommand>
{
    public AdminAddProductToCartCommandValidator()
    {
        RuleFor(x => x.EcommerceUserId)
            .NotEmpty().WithMessage("El Id del usuario ecommerce es requerido.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("El Id del producto debe ser mayor a 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.")
            .When(x => x.UnitPrice.HasValue);
    }
}
