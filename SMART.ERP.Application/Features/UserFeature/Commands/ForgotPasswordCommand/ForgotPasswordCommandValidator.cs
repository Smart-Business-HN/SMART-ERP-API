using FluentValidation;

namespace SMART.ERP.Application.Features.UserFeature.Commands.ForgotPasswordCommand
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} no debe ser vacio");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$")
                .WithMessage("La {PropertyName} debe tener mínimo ocho caracteres, al menos una letra mayúscula, una letra minúscula y un número");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}
