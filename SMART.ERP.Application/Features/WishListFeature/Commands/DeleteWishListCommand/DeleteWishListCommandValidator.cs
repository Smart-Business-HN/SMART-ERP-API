using FluentValidation;

namespace SMART.ERP.Application.Features.WishListFeature.Commands.DeleteWishListCommand
{
    internal class DeleteWishListCommandValidator : AbstractValidator<DeleteWishListCommand>
    {
        public DeleteWishListCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
