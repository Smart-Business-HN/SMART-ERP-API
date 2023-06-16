using FluentValidation;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.DeleteDataSheetCommand
{
    public class DeleteDataSheetCommandValidator : AbstractValidator<DeleteDataSheetCommand>
    {
        public DeleteDataSheetCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
