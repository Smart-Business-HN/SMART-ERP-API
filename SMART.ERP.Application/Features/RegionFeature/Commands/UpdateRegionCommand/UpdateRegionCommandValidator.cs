using FluentValidation;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.UpdateRegionCommand
{
    public class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
    {
        public UpdateRegionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
