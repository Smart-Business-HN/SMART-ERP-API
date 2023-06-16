using FluentValidation;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Commands.CreateTypeActivityCommand
{
    public class CreateTypeActivityCommandValidator : AbstractValidator<CreateTypeActivityCommand>
    {
        public CreateTypeActivityCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
