using FluentValidation;

namespace SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.CreateNonBillableExpensePaymentCommand
{
    public class CreateNonBillableExpensePaymentCommandValidator : AbstractValidator<CreateNonBillableExpensePaymentCommand>
    {
        public CreateNonBillableExpensePaymentCommandValidator()
        {
            RuleFor(x => x.Date)
                   .NotNull().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} no puede ser inferior o igual a cero")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.NonBillableExpenseId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.TypeOfPaymentMethodId)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
