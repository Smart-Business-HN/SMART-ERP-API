using FluentValidation;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.UpdateAssociatedCompanyCommand;

public class UpdateAssociatedCompanyCommandValidator : AbstractValidator<UpdateAssociatedCompanyCommand>
{
    public UpdateAssociatedCompanyCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("El Id es requerido.");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} no puede ser vacío.")
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.");

        RuleFor(p => p.RTN)
            .Matches(@"^\d{14}$").WithMessage("El RTN debe tener exactamente 14 dígitos numéricos.")
            .When(x => !string.IsNullOrEmpty(x.RTN));

        RuleFor(p => p.PhoneNumber)
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del teléfono. Ejemplo: 0000-0000")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(p => p.Address)
            .MaximumLength(300).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .When(x => x.Address != null);

        RuleFor(p => p.Email)
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .EmailAddress().WithMessage("{PropertyName} debe ser un correo válido.")
            .When(x => x.Email != null);
    }
}
