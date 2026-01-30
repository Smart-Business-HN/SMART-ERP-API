using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectFeature.Commands.UpdateProjectCommand
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(p => p.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo");
            RuleFor(p => p.ExecutionBudget)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} no puede ser negativo");
            RuleFor(p => p.StatusId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
