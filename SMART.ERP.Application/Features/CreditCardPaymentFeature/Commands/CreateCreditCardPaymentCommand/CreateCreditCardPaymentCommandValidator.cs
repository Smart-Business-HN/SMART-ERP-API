using FluentValidation;

namespace SMART.ERP.Application.Features.CreditCardPaymentFeature.Commands.CreateCreditCardPaymentCommand
{
    public class CreateCreditCardPaymentCommandValidator : AbstractValidator<CreateCreditCardPaymentCommand>
    {
        public CreateCreditCardPaymentCommandValidator()
        {
            RuleFor(x => x.CreditCardInternalBankAccountId).GreaterThan(0).WithMessage("La tarjeta de crédito es requerida.");
            RuleFor(x => x.SourceInternalBankAccountId).GreaterThan(0).WithMessage("La cuenta bancaria origen es requerida.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha es requerida.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
            RuleFor(x => x.Reference).MaximumLength(100);
            RuleFor(x => x.Notes).MaximumLength(500);
        }
    }
}
