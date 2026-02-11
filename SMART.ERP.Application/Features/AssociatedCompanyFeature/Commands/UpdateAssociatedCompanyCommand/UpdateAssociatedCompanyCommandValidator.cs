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
            .MaximumLength(20).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .When(x => x.RTN != null);

        RuleFor(p => p.PhoneNumber)
            .MaximumLength(20).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .When(x => x.PhoneNumber != null);

        RuleFor(p => p.Address)
            .MaximumLength(300).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .When(x => x.Address != null);

        RuleFor(p => p.Email)
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
            .EmailAddress().WithMessage("{PropertyName} debe ser un correo válido.")
            .When(x => x.Email != null);
    }
}
