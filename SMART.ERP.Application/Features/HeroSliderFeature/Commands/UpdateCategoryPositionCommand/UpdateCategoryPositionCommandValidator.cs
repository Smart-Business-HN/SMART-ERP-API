using FluentValidation;

namespace SMART.ERP.Application.Features.HeroSliderFeature.Commands.UpdateCategoryPositionCommand
{
    public class UpdateCategoryPositionCommandValidator : AbstractValidator<UpdateCategoryPositionCommand>
    {
        public UpdateCategoryPositionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} no debe ser vacio");
        }
    }
}
