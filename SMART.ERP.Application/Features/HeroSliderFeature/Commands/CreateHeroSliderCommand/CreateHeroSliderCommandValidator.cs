using FluentValidation;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.CreateHeroSliderCommand
{
    public class CreateHeroSliderCommandValidator : AbstractValidator<CreateHeroSliderCommand>
    {
        public CreateHeroSliderCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.ProductId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.Position)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero")
                .NotEqual(5).WithMessage("Solo se permiten 4 items por categoria");
        }
    }
}
