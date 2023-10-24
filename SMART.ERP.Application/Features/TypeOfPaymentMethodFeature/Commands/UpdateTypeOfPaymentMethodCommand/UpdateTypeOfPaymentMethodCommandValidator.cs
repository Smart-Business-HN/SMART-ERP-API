using FluentValidation;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.UpdateTypeOfPaymentMethodCommand
{
    public class UpdateTypeOfPaymentMethodCommandValidator : AbstractValidator<UpdateTypeOfPaymentMethodCommand>
    {
        public UpdateTypeOfPaymentMethodCommandValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName es requerido}");
        }
    }
}
