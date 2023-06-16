using System;
using FluentValidation;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.WishListFeature.Commands.UpdateWishListCommand
{
    public class UpdateWishListCommandValidator : AbstractValidator<UpdateWishListCommand>
    {
        public UpdateWishListCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.CustomerId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");

            RuleFor(x => x.ModificationDate)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.StatusId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotEqual(0).WithMessage("El {PropertyName} no puede ser cero");

            RuleFor(x => x.Code)
               .NotNull().WithMessage("{PropertyName} no puede ser nulo")
               .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}

