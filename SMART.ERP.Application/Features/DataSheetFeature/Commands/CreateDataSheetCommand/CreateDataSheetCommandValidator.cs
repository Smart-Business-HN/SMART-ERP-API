using FluentValidation;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.CreateDataSheetCommand
{
    public class CreateDataSheetCommandValidator : AbstractValidator<CreateDataSheetCommand>
    {
        public CreateDataSheetCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
