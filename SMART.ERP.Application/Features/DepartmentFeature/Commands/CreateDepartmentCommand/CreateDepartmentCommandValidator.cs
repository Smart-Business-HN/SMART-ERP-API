using FluentValidation;

namespace SMART.ERP.Application.Features.DepartmentFeature.Commands.CreateDepartmentCommand
{
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength}");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CountryId)
                .NotNull().WithMessage("{PropertyName} es requerido")
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
