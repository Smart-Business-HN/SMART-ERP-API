using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationItemObservationCommand
{
    public class CreateQuotationItemObservationCommandValidator : AbstractValidator<CreateQuotationItemObservationCommand>
    {
        public CreateQuotationItemObservationCommandValidator()
        {
            RuleFor(p => p.AccessToken)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.ProductOfferedId)
                .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a 0");

            RuleFor(p => p.AuthorName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Observation)
                .NotEmpty().WithMessage("{PropertyName} es requerida")
                .MaximumLength(500).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
