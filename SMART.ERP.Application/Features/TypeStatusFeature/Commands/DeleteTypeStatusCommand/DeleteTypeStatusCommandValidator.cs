using FluentValidation;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.DeleteTypeStatusCommand
{
    public class DeleteTypeStatusCommandValidator : AbstractValidator<DeleteTypeStatusCommand>
    {
        public DeleteTypeStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
