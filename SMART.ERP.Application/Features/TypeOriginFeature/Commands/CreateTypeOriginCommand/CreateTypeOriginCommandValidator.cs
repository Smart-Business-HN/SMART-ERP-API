using FluentValidation;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Commands.CreateTypeOriginCommand
{
    public class CreateTypeOriginCommandValidator : AbstractValidator<CreateTypeOriginCommand>
    {
        public CreateTypeOriginCommandValidator()
        {
            RuleFor(p => p.Name)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
            RuleFor(p => p.IsActive)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio");
            RuleFor(p => p.IsProspectOrigin)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio").NotNull().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
