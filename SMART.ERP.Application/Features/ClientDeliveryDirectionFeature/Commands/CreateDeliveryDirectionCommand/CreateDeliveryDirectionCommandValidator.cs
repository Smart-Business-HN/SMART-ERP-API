using FluentValidation;

namespace SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.CreateDeliveryDirectionCommand
{
    public class CreateDeliveryDirectionCommandValidator : AbstractValidator<CreateDeliveryDirectionCommand>
    {
        public CreateDeliveryDirectionCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .Matches(@"^\d{4}-\d{4}$").WithMessage("Verifique el formato del {PropertyName} Ejemplo: 0000-0000")
                .MaximumLength(15).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .MinimumLength(8).WithMessage("{PropertyName} debe ser mayor a {MinLength} caracteres");

            RuleFor(x => x.AdditionalInformation)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
