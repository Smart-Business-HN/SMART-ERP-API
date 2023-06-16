using FluentValidation;

namespace SMART.ERP.Application.Features.AdvisorDepartmentFeature.Commands.CreateAdvisorDepartmentCommand
{
    public class CreateAdvisorDepartmentCommandValidator : AbstractValidator<CreateAdvisorDepartmentCommand>
    {
        public CreateAdvisorDepartmentCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
