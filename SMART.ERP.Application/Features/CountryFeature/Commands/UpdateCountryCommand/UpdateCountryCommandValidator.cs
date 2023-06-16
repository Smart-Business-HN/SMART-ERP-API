using FluentValidation;

namespace SMART.ERP.Application.Features.CountryFeature.Commands.UpdateCountryCommand
{
    public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Abbreviation)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(10).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.PhoneNumberCode)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(20).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
