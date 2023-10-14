using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Commands.CreateBillPaymentCommand
{
    public class CreateBillPaymentCommandValidator : AbstractValidator<CreateBillPaymentCommand>
    {
        public CreateBillPaymentCommandValidator() {
            RuleFor(x => x.Date)
                   .NotNull().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} no puede ser inferior o igual a cero")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
            
            RuleFor(x => x.TypeOfPaymentMethodId)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido");

        }
    }
}
