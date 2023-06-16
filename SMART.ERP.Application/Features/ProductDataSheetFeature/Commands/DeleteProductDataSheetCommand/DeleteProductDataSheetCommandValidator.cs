using FluentValidation;

namespace SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.DeleteProductDataSheetCommand
{
    public class DeleteProductDataSheetCommandValidator : AbstractValidator<DeleteProductDataSheetCommand>
    {
        public DeleteProductDataSheetCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
