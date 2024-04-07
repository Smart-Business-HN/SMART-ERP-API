using FluentValidation;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.DeleteNonBillableExpenseCommand
{
    public class DeleteNonBillableExpenseValidator : AbstractValidator<DeleteNonBillableExpenseCommand>
    {
        public DeleteNonBillableExpenseValidator()
        {
            RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
            .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
