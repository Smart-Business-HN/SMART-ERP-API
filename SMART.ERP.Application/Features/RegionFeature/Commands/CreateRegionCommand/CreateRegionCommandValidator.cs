using FluentValidation;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.CreateRegionCommand
{
    public class CreateRegionCommandValidator : AbstractValidator<CreateRegionCommand>
    {
        public CreateRegionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
