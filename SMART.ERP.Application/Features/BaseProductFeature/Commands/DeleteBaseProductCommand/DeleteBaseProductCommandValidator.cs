using FluentValidation;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.DeleteBaseProductCommand
{
    public class DeleteBaseProductCommandValidator : AbstractValidator<DeleteBaseProductCommand>
    {
        public DeleteBaseProductCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
