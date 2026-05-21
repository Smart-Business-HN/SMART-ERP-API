using FluentValidation;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCreditCommand
{
    public class UpdateCustomerCreditCommandValidator : AbstractValidator<UpdateCustomerCreditCommand>
    {
        public UpdateCustomerCreditCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} no debe ser vacio");

            RuleFor(x => x.CreditLimit)
                .GreaterThan(0)
                .WithMessage("El límite de crédito debe ser mayor a 0 cuando el crédito está habilitado")
                .When(x => x.CreditEnabled);

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El límite de crédito no puede ser negativo")
                .When(x => !x.CreditEnabled);
        }
    }
}
