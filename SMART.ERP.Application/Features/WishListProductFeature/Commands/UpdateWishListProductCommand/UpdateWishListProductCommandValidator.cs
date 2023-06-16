using System;
using FluentValidation;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateQuoteProductCommand;

namespace SMART.ERP.Application.Features.WishListProductFeature.Commands.UpdateWishListProductCommand
{
    public class UpdateWishListProductCommandValidator : AbstractValidator<UpdateWishListProductCommand>
    {
        public UpdateWishListProductCommandValidator()
        {
            RuleFor(p => p.Id)
                  .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                  .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Quantity)
                   .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                   .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}

