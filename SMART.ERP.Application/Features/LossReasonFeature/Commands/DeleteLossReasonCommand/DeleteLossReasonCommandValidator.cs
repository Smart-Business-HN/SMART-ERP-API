using FluentValidation;
using SMART.ERP.Application.Features.WinReasonFeature.Commands.DeleteWinReasonCommand;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.DeleteLossReasonCommand
{
    public class DeleteLossReasonCommandValidator : AbstractValidator<DeleteWinReasonCommand>
    {
        public DeleteLossReasonCommandValidator()
        {
            RuleFor(p => p.Id)
               .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
               .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
