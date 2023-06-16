using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectFeature.Commands.CreateProspectXpressCommand
{
    public class CreateProspectXpressCommandValidator : AbstractValidator<CreateProspectXpressCommand>
    {
        public CreateProspectXpressCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");


        }
    }
}
