using FluentValidation;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.RemoveRegionDepartmentCommand
{
    public class RemoveRegionDepartmentCommandValidator : AbstractValidator<RemoveRegionDepartmentCommand>
    {
        public RemoveRegionDepartmentCommandValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
