using FluentValidation;

namespace SMART.ERP.Application.Features.CheckoutFeature.Commands.RequestPaymentLinkCommand
{
    public class RequestPaymentLinkCommandValidator : AbstractValidator<RequestPaymentLinkCommand>
    {
        public RequestPaymentLinkCommandValidator()
        {
            RuleFor(x => x.CartId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(200).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .EmailAddress().WithMessage("{PropertyName} debe ser un email válido");

            RuleFor(x => x.CustomerPhone)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
