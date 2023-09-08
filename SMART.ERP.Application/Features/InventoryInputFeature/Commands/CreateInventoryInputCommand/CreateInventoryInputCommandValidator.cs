using FluentValidation;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommand
{
    public class CreateInventoryInputCommandValidator : AbstractValidator<CreateInventoryInputCommand>
    {
        public CreateInventoryInputCommandValidator()
        {
            RuleFor(p => p.InventoryInputTypeId)
                .NotNull().WithMessage("{PropertyName} es requerida");
            RuleFor(p => p.WarehouseId)
               .NotNull().WithMessage("{PropertyName} es requerida");
            RuleFor(p => p.PrefixId)
               .NotNull().WithMessage("{PropertyName} es requerida");
        }
    }
}
