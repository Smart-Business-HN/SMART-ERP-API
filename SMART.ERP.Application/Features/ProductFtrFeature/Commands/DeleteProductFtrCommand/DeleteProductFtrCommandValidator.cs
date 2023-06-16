using FluentValidation;

namespace SMART.ERP.Application.Features.ProductFtrFeature.Commands.DeleteProductFtrCommand
{
    public class DeleteProductFtrCommandValidator : AbstractValidator<DeleteProductFtrCommand>
    {
        public DeleteProductFtrCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
