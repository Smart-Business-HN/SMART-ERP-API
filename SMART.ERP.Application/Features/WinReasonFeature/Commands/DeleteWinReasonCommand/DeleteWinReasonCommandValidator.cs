using FluentValidation;

namespace SMART.ERP.Application.Features.WinReasonFeature.Commands.DeleteWinReasonCommand
{
    public class DeleteWinReasonCommandValidator : AbstractValidator<DeleteWinReasonCommand>
    {
        public DeleteWinReasonCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
