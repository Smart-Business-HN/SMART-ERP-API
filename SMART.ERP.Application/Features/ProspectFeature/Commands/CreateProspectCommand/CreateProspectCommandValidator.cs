using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectCommand
{
    public class CreateProspectCommandValidator : AbstractValidator<CreateProspectCommand>
    {
        public CreateProspectCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.Email)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^([(][0-9]{1,3}[)][\s][0-9]{3}[-][0-9]{4})|([0-9]{4}-[0-9]{4})$").WithMessage("Verifique el formato del {PropertyName}. Ejemplo: 0000-0000 para numeros locales." +
                "(000) 000-0000 para numeros extranjeros")
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.PreferredContactMethod)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.PreferredContactMethod));

            RuleFor(x => x.ContactPerson)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.ContactPerson));

            RuleFor(x => x.ContactPersonPhone)
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.ContactPersonPhone));

            RuleFor(x => x.ContactPersonEmail)
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.ContactPersonEmail));

            RuleFor(x => x.Address)
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.SocialReasonId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.HeadingId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.TypeOriginId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.Products)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
