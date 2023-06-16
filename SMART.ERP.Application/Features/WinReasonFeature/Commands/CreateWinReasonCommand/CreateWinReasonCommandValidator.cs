using FluentValidation;

namespace SMART.ERP.Application.Features.WinReasonFeature.Commands.CreateWinReasonCommand
{
    public class CreateWinReasonCommandValidator : AbstractValidator<CreateWinReasonCommand>
    {
        public CreateWinReasonCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
