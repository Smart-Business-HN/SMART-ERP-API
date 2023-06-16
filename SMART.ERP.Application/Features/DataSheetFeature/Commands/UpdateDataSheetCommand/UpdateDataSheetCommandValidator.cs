using FluentValidation;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.UpdateDataSheetCommand
{
    public class UpdateDataSheetCommandValidator : AbstractValidator<UpdateDataSheetCommand>
    {
        public UpdateDataSheetCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
