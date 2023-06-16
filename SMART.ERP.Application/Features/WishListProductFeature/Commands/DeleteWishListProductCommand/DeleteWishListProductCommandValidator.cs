using FluentValidation;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.DeleteWishListProductCommand
{
    public class DeleteWishListProductCommandValidator : AbstractValidator<DeleteWishListProductCommand>
    {
        public DeleteWishListProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
