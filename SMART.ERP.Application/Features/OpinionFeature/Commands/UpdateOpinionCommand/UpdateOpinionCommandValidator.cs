using FluentValidation;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.UpdateOpinionCommand
{
    public class UpdateOpinionCommandValidator : AbstractValidator<UpdateOpinionCommand>
    {
        public UpdateOpinionCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Customer)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Charge)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Observation)
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio")
                .MaximumLength(300).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
