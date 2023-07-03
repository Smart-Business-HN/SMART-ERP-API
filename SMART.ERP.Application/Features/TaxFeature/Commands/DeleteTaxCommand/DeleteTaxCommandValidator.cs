using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.DeleteTaxCommand
{
    public class DeleteTaxCommandValidator : AbstractValidator<DeleteTaxCommand>
    {
        public DeleteTaxCommandValidator() {
            RuleFor(p => p.Id)
                   .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                   .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
