using FluentValidation;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.CreateBranchOfficeCommand
{
    public class CreateBranchOfficeCommandValidator : AbstractValidator<CreateBranchOfficeCommand>
    {
        public CreateBranchOfficeCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Address)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .EmailAddress().WithMessage("Verfique el formato de {PropertyName}")
                .MaximumLength(30).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000");
        }
    }
}
