using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;

public class CreateEcommerUserValidator : AbstractValidator<CreateEcommerceUserCommand>
{
    public CreateEcommerUserValidator()
    {
        RuleFor(x=>x.Email).NotEmpty().WithMessage("El Email es requerido.");
        RuleFor(x=>x.Email).EmailAddress().WithMessage("El Email no es valido.");
        RuleFor(x=>x.Password).NotEmpty().WithMessage("La Contraseña es requerida.");
        RuleFor(x=>x.FirstName).NotEmpty().WithMessage("Primer Nombre es requerido.");
    }
}