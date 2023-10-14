using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.BillPaymentFeature.Commands.DeleteBillPaymentCommand
{
    public class DeleteBillPaymentCommandValidator : AbstractValidator<DeleteBillPaymentCommand>
    {
        public DeleteBillPaymentCommandValidator() {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
