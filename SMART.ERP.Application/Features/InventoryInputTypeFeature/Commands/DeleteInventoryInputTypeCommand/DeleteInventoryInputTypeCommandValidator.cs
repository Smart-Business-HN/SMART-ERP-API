using FluentValidation;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.DeleteInventoryInputTypeCommand
{
    public class DeleteInventoryInputTypeCommandValidator : AbstractValidator<DeleteInventoryInputTypeCommand>
    {
        public DeleteInventoryInputTypeCommandValidator()
        {
            RuleFor(p => p.Id)
                  .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                  .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
