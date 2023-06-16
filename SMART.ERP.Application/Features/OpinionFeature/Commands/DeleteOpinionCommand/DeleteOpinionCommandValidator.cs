using FluentValidation;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.DeleteOpinionCommand
{
    public class DeleteOpinionCommandValidator : AbstractValidator<DeleteOpinionCommand>
    {
        public DeleteOpinionCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
