using FluentValidation;

namespace SMART.ERP.Application.Features.AdvisorGoalFeature.Commands.UpdateAdvisorGoalCommand
{
    public class UpdateAdvisorGoalCommandValidator : AbstractValidator<UpdateAdvisorGoalCommand>
    {
        public UpdateAdvisorGoalCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.Goals)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleForEach(x => x.Goals)
                .ChildRules(goal =>
                {
                    goal.RuleFor(y => y.Goal)
                        .NotNull().WithMessage("{PropertyName} es requerido")
                        .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a cero");
                });
        }
    }
}
