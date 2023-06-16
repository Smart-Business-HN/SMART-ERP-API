using FluentValidation;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateHeroSliderCommand
{
    public class UpdateHeroSliderCommandValidator : AbstractValidator<UpdateHeroSliderCommand>
    {
        public UpdateHeroSliderCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.Position)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero")
                .NotEqual(5).WithMessage("Solo se permiten 4 items por categoria");
        }
    }
}
