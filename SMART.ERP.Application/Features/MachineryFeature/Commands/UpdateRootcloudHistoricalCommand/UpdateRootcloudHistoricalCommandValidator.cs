using FluentValidation;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateRootcloudHistoricalCommand
{
    public class UpdateRootcloudHistoricalCommandValidator : AbstractValidator<UpdateRootcloudHistoricalCommand>
    {
        public UpdateRootcloudHistoricalCommandValidator()
        {
            RuleFor(p => p.Id)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
