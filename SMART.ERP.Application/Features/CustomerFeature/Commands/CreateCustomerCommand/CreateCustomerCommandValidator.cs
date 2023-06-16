using FluentValidation;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.CreateCustomerCommand
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(p => p.FirstName)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.LastName)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.FullName)
                .MaximumLength(120).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Verifique el formato de {PropertyName}")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(p => p.PhoneNumber)
                .Matches(@"^([(][0-9]{1,3}[)][\s][0-9]{3}[-][0-9]{4})|([0-9]{4}-[0-9]{4})$").WithMessage("Verifique el formato del {PropertyName}. Ejemplo: 0000-0000 para numeros locales" +
                "(000) 000-0000 para numeros extranjeros")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(8).WithMessage("{PropertyName} debe ser mayor a {MinLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(p => p.SocialReasonId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.CountryId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

        }
    }
}