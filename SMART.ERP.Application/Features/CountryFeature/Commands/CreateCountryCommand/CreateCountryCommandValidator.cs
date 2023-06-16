using FluentValidation;

namespace SMART.ERP.Application.Features.CountryFeature.Commands.CreateCountryCommand
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        public CreateCountryCommandValidator()
        {
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
