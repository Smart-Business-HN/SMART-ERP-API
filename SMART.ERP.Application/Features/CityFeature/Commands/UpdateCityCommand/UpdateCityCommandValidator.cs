using FluentValidation;

namespace SMART.ERP.Application.Features.CityFeature.Commands.UpdateCityCommand
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName es requerido}");
        }
    }
}
