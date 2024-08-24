using FluentValidation;

namespace SMART.ERP.Application.Features.DiscountFeature.Commands.CreateDiscountCommand
{
    public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength}");

            RuleFor(x => x.Rate)
                .NotNull().WithMessage("{PropertyName} es requerido");

        }
    }
}

