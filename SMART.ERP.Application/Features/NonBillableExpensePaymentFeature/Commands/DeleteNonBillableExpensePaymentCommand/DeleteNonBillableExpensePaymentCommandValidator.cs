using FluentValidation;

namespace SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.DeleteNonBillableExpensePaymentCommand
{
    public class DeleteNonBillableExpensePaymentCommandValidator : AbstractValidator<DeleteNonBillableExpensePaymentCommand>
    {
        public DeleteNonBillableExpensePaymentCommandValidator()
        {
            RuleFor(x => x.Id)
                  .NotEmpty().WithMessage("{PropertyName} es requerido")
                  .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
