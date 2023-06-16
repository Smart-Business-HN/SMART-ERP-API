using FluentValidation;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.DeleteHeroSliderCommand
{
    public class DeleteHeroSliderCommandValidator : AbstractValidator<DeleteHeroSliderCommand>
    {
        public DeleteHeroSliderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} no debe ser cero");
        }
    }
}
