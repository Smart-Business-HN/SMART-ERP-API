using FluentValidation;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Commands.UpdatePaymentMethodCommand;

public class UpdatePaymentMethodCommandValidator : AbstractValidator<UpdatePaymentMethodCommand>
{
    public UpdatePaymentMethodCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("El Id es requerido.");

        RuleFor(p => p.Alias)
            .NotEmpty().WithMessage("{PropertyName} no puede ser vacío.")
            .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.");

        RuleFor(p => p.CardholderName)
            .NotEmpty().WithMessage("El nombre del titular no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre del titular no debe exceder {MaxLength} caracteres.");

        RuleFor(p => p.ExpirationMonth)
            .InclusiveBetween(1, 12).WithMessage("El mes de expiración debe estar entre 1 y 12.");

        RuleFor(p => p.ExpirationYear)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("El año de expiración no puede ser menor al año actual.");

        RuleFor(p => p)
            .Must(p => !IsExpired(p.ExpirationMonth, p.ExpirationYear))
            .WithMessage("La tarjeta está vencida.")
            .When(p => p.ExpirationMonth >= 1 && p.ExpirationMonth <= 12 && p.ExpirationYear >= DateTime.UtcNow.Year);
    }

    private static bool IsExpired(int month, int year)
    {
        var now = DateTime.UtcNow;
        return year < now.Year || (year == now.Year && month < now.Month);
    }
}
