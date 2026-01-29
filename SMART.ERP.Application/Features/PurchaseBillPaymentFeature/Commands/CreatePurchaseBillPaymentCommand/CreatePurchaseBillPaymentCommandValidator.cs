using FluentValidation;

namespace SMART.ERP.Application.Features.PurchaseBillPaymentFeature.Commands.CreatePurchaseBillPaymentCommand
{
    public class CreatePurchaseBillPaymentCommandValidator : AbstractValidator<CreatePurchaseBillPaymentCommand>
    {
        public CreatePurchaseBillPaymentCommandValidator()
        {
            RuleFor(x => x.Date)
                  .NotNull().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("{PropertyName} no puede ser inferior o igual a cero")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.PurchaseBillId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.TypeOfPaymentMethodId)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
