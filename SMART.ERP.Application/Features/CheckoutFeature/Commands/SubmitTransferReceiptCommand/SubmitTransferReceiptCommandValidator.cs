using FluentValidation;

namespace SMART.ERP.Application.Features.CheckoutFeature.Commands.SubmitTransferReceiptCommand
{
    public class SubmitTransferReceiptCommandValidator : AbstractValidator<SubmitTransferReceiptCommand>
    {
        public SubmitTransferReceiptCommandValidator()
        {
            RuleFor(x => x.CartId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .EmailAddress().WithMessage("{PropertyName} debe ser un email válido");

            RuleFor(x => x.BankName)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.Receipt)
                .NotNull().WithMessage("El comprobante es requerido");
        }
    }
}
