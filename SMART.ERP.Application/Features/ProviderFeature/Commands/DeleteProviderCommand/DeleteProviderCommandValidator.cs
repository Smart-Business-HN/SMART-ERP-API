using FluentValidation;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.DeleteProviderCommand
{
    public class DeleteProviderCommandValidator : AbstractValidator<DeleteProviderCommand>
    {
        public DeleteProviderCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
