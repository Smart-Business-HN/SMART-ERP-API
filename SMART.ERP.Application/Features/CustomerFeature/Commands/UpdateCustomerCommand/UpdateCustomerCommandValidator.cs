using FluentValidation;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} no debe ser vacio");

            RuleFor(p => p.FirstName)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.LastName)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.FullName)
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Verifique el formato de {PropertyName}")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.DNI)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.DNI));

            RuleFor(x => x.RTN)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.RTN));

            RuleFor(x => x.Company)
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Company));

            RuleFor(x => x.SecondaryEmail)
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));

            RuleFor(x => x.SecondaryPhoneNumber)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.SecondaryPhoneNumber));

            RuleFor(x => x.CivilStatus)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.CivilStatus));

            RuleFor(x => x.ContactPerson)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));

            RuleFor(x => x.ContactPersonPhone)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPersonPhone));

            RuleFor(x => x.ContactPersonEmail)
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPersonEmail));

            RuleFor(p => p.PhoneNumber)
                .Matches(@"^([(][0-9]{1,3}[)][\s][0-9]{3}[-][0-9]{4})|([0-9]{4}-[0-9]{4})$").WithMessage("Verifique el formato del {PropertyName}. Ejemplo: 0000-0000 para numeros locales" +
                "(000) 000-0000 para numeros extranjeros")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(8).WithMessage("{PropertyName} debe ser mayor a {MinLength} caracteres");
        }
    }
}
