using FluentValidation;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.CreateInventoryInputTypeCommand
{
    public class CreateInventoryInputTypeCommandValidator : AbstractValidator<CreateInventoryInputTypeCommand>
    {
        public CreateInventoryInputTypeCommandValidator() {
            RuleFor(p => p.Name)
                    .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                    .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.IsActive)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
