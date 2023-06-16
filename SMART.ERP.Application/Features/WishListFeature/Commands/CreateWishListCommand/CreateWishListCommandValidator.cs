using FluentValidation;

namespace SMART.ERP.Application.Features.WishListFeature.Commands.CreateWishListCommand
{
    public class CreateWishListCommandValidator : AbstractValidator<CreateWishListCommand>
    {

        public CreateWishListCommandValidator()
        {
            RuleFor(p => p.CustomerId)
               .NotNull().WithMessage("{PropertyName} no puede ser nulo")
               .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");

        }
    }
}
