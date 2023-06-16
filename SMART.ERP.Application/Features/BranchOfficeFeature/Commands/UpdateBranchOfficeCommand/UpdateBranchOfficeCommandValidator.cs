using FluentValidation;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.UpdateBranchOfficeCommand
{
    public class UpdateBranchOfficeCommandValidator : AbstractValidator<UpdateBranchOfficeCommand>
    {
        public UpdateBranchOfficeCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

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
