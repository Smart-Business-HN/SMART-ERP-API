using FluentValidation;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.CreateTypeStatusCommand
{
    public class CreateTypeStatusCommandValidator : AbstractValidator<CreateTypeStatusCommand>
    {
        public CreateTypeStatusCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
