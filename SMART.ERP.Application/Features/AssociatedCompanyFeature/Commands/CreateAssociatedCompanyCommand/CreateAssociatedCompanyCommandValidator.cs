using FluentValidation;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.CreateAssociatedCompanyCommand;

public class CreateAssociatedCompanyCommandValidator : AbstractValidator<CreateAssociatedCompanyCommand>
{
    public CreateAssociatedCompanyCommandValidator()
    {
        RuleFor(p => p.EcommerceUserId)
            .NotEmpty().WithMessage("El Id del usuario es requerido.");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} no puede ser vacío.")
            .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.");

        RuleFor(p => p.RTN)
            .MaximumLength(14).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres.")
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
