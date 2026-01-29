using FluentValidation;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.DeleteTypeOfPaymentMethodCommand
{
    public class DeleteTypeOfPaymentMethodCommandValidator : AbstractValidator<DeleteTypeOfPaymentMethodCommand>
    {
        public DeleteTypeOfPaymentMethodCommandValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
