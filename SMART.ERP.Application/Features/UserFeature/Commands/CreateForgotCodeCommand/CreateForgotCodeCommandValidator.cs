using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.CreateForgotCodeCommand
{
    public class CreateForgotCodeCommandValidator : AbstractValidator<CreateForgotCodeCommand>
    {
        public CreateForgotCodeCommandValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}");
        }
    }
}
