using FluentValidation;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.DeleteInternalBankAccountCommand
{
    public class DeleteInternalBankAccountCommandValidator : AbstractValidator<DeleteInternalBankAccountCommand>
    {
        public DeleteInternalBankAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                   .NotEmpty().WithMessage("{PropertyName} es requerido")
                   .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
