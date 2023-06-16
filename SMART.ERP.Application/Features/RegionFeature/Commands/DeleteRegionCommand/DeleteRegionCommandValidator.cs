using FluentValidation;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.DeleteRegionCommand
{
    public class DeleteRegionCommandValidator : AbstractValidator<DeleteRegionCommand>
    {
        public DeleteRegionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
