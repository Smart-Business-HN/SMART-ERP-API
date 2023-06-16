using FluentValidation;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.AssignRegionDepartmentCommand
{
    public class AssignRegionDepartmentCommandValidator : AbstractValidator<AssignRegionDepartmentCommand>
    {
        public AssignRegionDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.Departments)
                .NotEmpty().WithMessage("{PropertyName} debe contener al menos un departamento");

            RuleForEach(x => x.Departments)
                .NotEmpty().WithMessage("El departamento {CollectionIndex} es requerido");
        }
    }
}
