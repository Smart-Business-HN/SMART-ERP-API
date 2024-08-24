using FluentValidation;

namespace SMART.ERP.Application.Features.DiscountFeature.Commands.DeleteDiscountCommand
{
    public class DeleteDiscountCommandValidator : AbstractValidator<DeleteDiscountCommand>
    
    {
        public DeleteDiscountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}