using FluentValidation;
using SMART.ERP.Application.Features.WinReasonFeature.Commands.UpdateWinReasonCommand;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.UpdateLossReasonCommand
{
    public class UpdateLossReasonCommandValidator : AbstractValidator<UpdateWinReasonCommand>
    {
        public UpdateLossReasonCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
