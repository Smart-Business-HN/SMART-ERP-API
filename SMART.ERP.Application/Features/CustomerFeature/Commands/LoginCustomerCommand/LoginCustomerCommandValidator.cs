using FluentValidation;

namespace SMART.ERP.Application.Features.CustomerFeature.Commands.LoginCustomerCommand
{
    public class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
    {
        public LoginCustomerCommandValidator()
        {
            RuleFor(x => x.Email)
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
