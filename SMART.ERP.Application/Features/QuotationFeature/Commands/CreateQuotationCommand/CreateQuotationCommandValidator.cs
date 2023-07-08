using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.CreateQuotationCommand
{
    public class CreateQuotationCommandValidator : AbstractValidator<CreateQuotationCommand>
    {
        public CreateQuotationCommandValidator() {
            RuleFor(p => p.CustomerId)
                   .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.BranchOfficeId)
                    .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.CreationDate)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo");
            RuleFor(p => p.DueDate)
                  .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(p => p.StatusId)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.PrefixId)
                    .NotNull().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}
