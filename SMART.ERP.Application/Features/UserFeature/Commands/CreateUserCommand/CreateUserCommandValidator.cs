using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.CreateUserCommand
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}");

            //RuleFor(p => p.Password)
            //    .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
            //    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\p{L}\p{N}[:punct:]]{8,}$")
            //    .WithMessage("La {PropertyName} debe tener mínimo ocho caracteres, al menos una letra mayúscula, una letra minúscula y un número");

            RuleFor(p => p.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(8).WithMessage("{PropertyName} debe ser mayor a {MinLength} caracteres");

            RuleFor(p => p.RoleId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(x => x.BranchOfficeId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .When(x => x.BranchOfficeId != null);

            RuleFor(p => p.GenderId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.CommissionPercentage)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje de comision debe estar entre 0 y 100")
                .When(p => p.CommissionPercentage.HasValue);
        }
    }
}
