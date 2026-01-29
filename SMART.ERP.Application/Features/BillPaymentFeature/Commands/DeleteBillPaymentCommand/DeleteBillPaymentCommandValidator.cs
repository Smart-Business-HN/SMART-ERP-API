using FluentValidation;

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
