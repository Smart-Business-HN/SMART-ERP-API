using FluentValidation;

namespace SMART.ERP.Application.Features.StatusFeature.Commands.DeleteStatusCommand
{
    public class DeleteStatusCommandValidator : AbstractValidator<DeleteStatusCommand>
    {
        public DeleteStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
