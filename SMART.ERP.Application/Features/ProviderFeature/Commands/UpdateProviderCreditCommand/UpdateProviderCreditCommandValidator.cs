using FluentValidation;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCreditCommand
{
    public class UpdateProviderCreditCommandValidator : AbstractValidator<UpdateProviderCreditCommand>
    {
        public UpdateProviderCreditCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("{PropertyName} no debe ser vacio");

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
