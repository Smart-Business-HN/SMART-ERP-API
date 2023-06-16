using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunitySchedulesFeature.Commands.UpdateOpportunityScheduleCommand
{
    public class UpdateOpportunityScheduleCommandValidator : AbstractValidator<UpdateOpportunityScheduleCommand>
    {
        public UpdateOpportunityScheduleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Option)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
