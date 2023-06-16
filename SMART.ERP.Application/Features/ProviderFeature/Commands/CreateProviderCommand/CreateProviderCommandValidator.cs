using FluentValidation;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.CreateProviderCommand
{
    public class CreateProviderCommandValidator : AbstractValidator<CreateProviderCommand>
    {
        public CreateProviderCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.RTN)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .Matches(@"^\d{4}-\d{4}-\d{6}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000-000000")
                .MaximumLength(16).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}");

            RuleFor(p => p.ContactPerson)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.ContactPerson));

            RuleFor(p => p.ContactPhoneNumber)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000")
                .When(x => !string.IsNullOrEmpty(x.ContactPhoneNumber));

            RuleFor(p => p.ContactEmail)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}")
                .When(x => !string.IsNullOrEmpty(x.ContactEmail));

            RuleFor(p => p.Address)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");
        }
    }
}
