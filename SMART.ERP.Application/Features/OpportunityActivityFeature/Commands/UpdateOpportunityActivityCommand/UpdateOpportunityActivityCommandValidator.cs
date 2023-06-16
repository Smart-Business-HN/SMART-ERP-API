using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.UpdateOpportunityActivityCommand
{
    public class UpdateOpportunityActivityCommandValidator : AbstractValidator<UpdateOpportunityActivityCommand>
    {
        public UpdateOpportunityActivityCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(300).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.InitDate)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .LessThanOrEqualTo(p => p.EndDate).WithMessage("La fecha inicial no puede ser mayor a la fecha final");

            RuleFor(p => p.TypeActivityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.StatusId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.OpportunityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
