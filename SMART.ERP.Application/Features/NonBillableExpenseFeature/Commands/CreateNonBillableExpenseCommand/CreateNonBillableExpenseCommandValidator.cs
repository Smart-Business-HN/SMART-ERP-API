using FluentValidation;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.CreateNonBillableExpenseCommand
{
    public class CreateNonBillableExpenseCommandValidator : AbstractValidator<CreateNonBillableExpenseCommand>
    {
        public CreateNonBillableExpenseCommandValidator()
        {
            RuleFor(x => x.Description)
                             .NotEmpty().WithMessage("{PropertyName} es requerido")
                             .NotNull().WithMessage("{PropertyName} es requerido")
                             .MaximumLength(250).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(p => p.ProviderId)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(p => p.PrefixId)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(p => p.Date)
              .NotNull().WithMessage("{PropertyName} no puede ser nulo");
            RuleFor(p => p.Amount)
              .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
              .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
