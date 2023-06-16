using FluentValidation;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Commands.DeleteTypeOriginCommand
{
    public class DeleteTypeOriginCommandValidator : AbstractValidator<DeleteTypeOriginCommand>
    {
        public DeleteTypeOriginCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty()
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
