using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Commands.DeleteSaleOrderCommand
{
    public class DeleteSaleOrderCommandValidator : AbstractValidator<DeleteSaleOrderCommand>
    {
        public DeleteSaleOrderCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
