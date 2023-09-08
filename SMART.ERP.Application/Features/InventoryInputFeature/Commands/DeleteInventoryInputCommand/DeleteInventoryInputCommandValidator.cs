using FluentValidation;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.DeleteInventoryInputCommand
{
    public class DeleteInventoryInputCommandValidator : AbstractValidator<DeleteInventoryInputCommand>
    {
        public DeleteInventoryInputCommandValidator()
        {
            RuleFor(p => p.Id)
                     .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                     .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
