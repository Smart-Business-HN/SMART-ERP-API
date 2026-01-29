using FluentValidation;

namespace SMART.ERP.Application.Features.TypeOfPaymentMethodFeature.Commands.CreateTypeOfPaymentMethodCommand
{
    public class CreateTypeOfPaymentMethodCommandValidator : AbstractValidator<CreateTypeOfPaymentMethodCommand>
    {
        public CreateTypeOfPaymentMethodCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.IsActive)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
