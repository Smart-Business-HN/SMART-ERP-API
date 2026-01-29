using FluentValidation;

namespace SMART.ERP.Application.Features.BankFeature.Commands.DeleteBankCommand
{
    public class DeleteBankValidator : AbstractValidator<DeleteBankCommand>
    {
        public DeleteBankValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

        }
    }
}
