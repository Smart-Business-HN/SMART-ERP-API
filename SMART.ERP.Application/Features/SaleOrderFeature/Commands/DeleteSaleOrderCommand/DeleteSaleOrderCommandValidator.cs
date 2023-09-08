using FluentValidation;

namespace SMART.ERP.Application.Features.SaleOrderFeature.Commands.DeleteSaleOrderCommand
{
    public class DeleteSaleOrderCommandValidator : AbstractValidator<DeleteSaleOrderCommand>
    {
        public DeleteSaleOrderCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
