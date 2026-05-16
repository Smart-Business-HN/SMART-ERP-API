using FluentValidation;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.CreateInternalBankAccountCommand
{
    public class CreateInternalBankAccountCommandValidator : AbstractValidator<CreateInternalBankAccountCommand>
    {
        public CreateInternalBankAccountCommandValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido")
               .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.BankId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("Tipo de cuenta inválido");

            RuleFor(x => x.CardLastFour)
                .NotEmpty().WithMessage("Los últimos 4 dígitos de la tarjeta son requeridos")
                .Length(4).WithMessage("Deben ser exactamente 4 dígitos")
                .Matches("^[0-9]{4}$").WithMessage("Sólo se permiten dígitos numéricos")
                .When(x => x.AccountType == InternalBankAccountType.CreditCard);
        }
    }
}
