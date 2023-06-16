using FluentValidation;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.CreateOpinionCommand
{
    public class CreateOpinionCommandValidator : AbstractValidator<CreateOpinionCommand>
    {
        public CreateOpinionCommandValidator()
        {
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
