using FluentValidation;

namespace SMART.ERP.Application.Features.ProductImageFeature.Commands.DeleteProductImageCommand
{
    public class DeleteProductImageCommandValidator : AbstractValidator<DeleteProductImageCommand>
    {
        public DeleteProductImageCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
