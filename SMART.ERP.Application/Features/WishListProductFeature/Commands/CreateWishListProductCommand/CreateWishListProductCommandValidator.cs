using FluentValidation;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.CreateWishListProductCommand
{
    public class CreateWishListProductCommandValidator : AbstractValidator<CreateWishListProductCommand>
    {
        public CreateWishListProductCommandValidator()
        {
            RuleFor(p => p.ProductId)
              .NotNull().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(p => p.WishListId)
             .NotNull().WithMessage("{PropertyName} no puede ser nulo")
             .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
